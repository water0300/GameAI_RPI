using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class MapData {
    
    [field: SerializeField] public List<Block> MapBlockList {get; set; } = new List<Block>();
    [field: SerializeField] public MapDimensions Dimensions {get; set; }

    public List<GraphNode> TileAdjList = new List<GraphNode>();
    public List<GraphNode> WaypointAdjList = new List<GraphNode>();
    //atm:
        //manually add to this list
        //disable parent
    public Block MapBlockGet(int row, int col){
        return MapBlockList[row * Dimensions.width + col];
    }

    public void GenerateTiles(int tilesize, GraphNode nodePrefab, Transform nodeParent, LineRenderer linePrefab){

        int rows = Mathf.CeilToInt((float) Dimensions.height / (float) tilesize);
        int cols = Mathf.CeilToInt((float) Dimensions.width / (float) tilesize);
        Debug.Log($"Expected dimensions: {rows} rows, {cols} cols, {rows * cols} total (possible) nodes");
        GraphNode[,] nodeMatrix = new GraphNode[rows,cols];

        int walkable = 0;
        int notwalk = 0;
        for(int offsetRow = 0; offsetRow < Dimensions.height; offsetRow += tilesize){
            for(int offsetCol = 0; offsetCol < Dimensions.width; offsetCol += tilesize){
                // Debug.Log($"Curr Offset: {offsetRow}, {offsetCol}, < {Dimensions.height}, {Dimensions.width}");

                //only get the node if walkable
                if(IsWalkable(offsetRow, offsetCol, tilesize)){
                    //get average position
                    Vector2 nodePos = Vector2.zero;
                    int blockCounter = 0;
                    for(int k = offsetRow; k < Dimensions.height && k - offsetRow < tilesize; k++){
                        for(int l = offsetCol; l < Dimensions.width && l - offsetCol < tilesize; l++){
                            // Debug.Log(MapBlockGet(k, l).transform.position.IgnoreZ());
                            nodePos += MapBlockGet(k, l).transform.position.IgnoreZ();
                            blockCounter++;
                        } 
                    }
                    nodePos /= blockCounter;

                    GraphNode node = Object.Instantiate(nodePrefab, nodePos, Quaternion.identity);
                    node.name = $"GraphNode: {offsetRow}x{offsetCol}";
                    node.transform.parent = nodeParent;
                    nodeMatrix[offsetRow/tilesize, offsetCol/tilesize] = node;
                    walkable++;
                } else {
                    notwalk++;
                }

                // return;
            }
            // return;
        }
        Debug.Log($"walkable: {walkable}, not: {notwalk}");

        //setup the adj list
        SetupTileAdjList(nodeMatrix, linePrefab, nodeParent);

    }

    private static (int, int)[] directions = new (int, int)[]{
        // (-1,-1),
        (-1,0),
        // (-1,1),
        (0,-1),
        (0,1),
        // (1,-1),
        (1,0),
        // (1,1)
    };

    
    public List<(Vector3, Vector3)> edges = new List<(Vector3, Vector3)>();
    private void SetupTileAdjList(GraphNode[,] nodeMatrix, LineRenderer linePrefab, Transform parent){
        for(int i = 0; i < nodeMatrix.GetLength(0); i++){
            for(int j = 0; j < nodeMatrix.GetLength(1); j++){
                //check all 8 directions
                if(nodeMatrix[i,j] == null){
                    continue;
                }

                TileAdjList.Add(nodeMatrix[i,j]);

                List<GraphNode> neighborNodes = new List<GraphNode>();
                foreach((int, int) direction in directions){
                    int row = direction.Item1 + i;
                    if(row < 0 || row >= nodeMatrix.GetLength(0)){
                        continue;
                    }

                    int col = direction.Item2 + j;
                    if(col < 0 || col >= nodeMatrix.GetLength(1)){
                        continue;
                    }

                    if(nodeMatrix[row,col] == null){
                        continue;
                    }

                    //established a connection
                    nodeMatrix[i,j].Children.Add(nodeMatrix[row, col]);
                    edges.Add((nodeMatrix[i,j].transform.position, nodeMatrix[row, col].transform.position));
                    //draw
                    // DrawLine(nodeMatrix[i,j].transform.position, nodeMatrix[row, col].transform.position, linePrefab, parent);

                }

            }
        }
    }
    private bool IsWalkable(int offsetRow, int offsetCol, int tilesize){
        // Debug.Log($"Curr Offset: {rowOffset}, {colOffset}, con: {rowOffset + tilesize}, {colOffset + tilesize}");
        int walkable = 0;
        int notwalkable = 0;
        for(int i = offsetRow; i < Dimensions.height && i - offsetRow < tilesize; i++){
            for(int j = offsetCol; j < Dimensions.width && j - offsetCol < tilesize; j++){
                // Debug.Log($"Check Offset: {i}, {j}, < {Dimensions.height}, {Dimensions.height}");
                if(!MapBlockGet(i, j).isWalkable){
                    notwalkable++;
                } else {
                    walkable++;
                }
            }
        }

        return walkable > notwalkable;
    }


    public void ResetGH(){
        foreach(GraphNode node in TileAdjList){
            node.G = -1;
            node.H = -1;
            node.IsPath = false;
        }
        foreach(GraphNode node in WaypointAdjList){
            node.G = -1;
            node.H = -1;
            node.IsPath = false;
        }
    }

    private void DrawLine(Vector3 startPos, Vector3 endPos, LineRenderer linePrefab, Transform parent){
        LineRenderer line = Object.Instantiate(linePrefab);
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
        line.widthMultiplier = 0.2f;
        line.transform.parent = parent;
    }

    public float lookahead = 1.5f;
    public float range = 4f;

    public void SetupWaypointAdjList(Transform parent, LineRenderer linePrefab){
        Physics2D.queriesStartInColliders = false;
        foreach(GraphNode node in WaypointAdjList){
            Collider2D[] cols = Physics2D.OverlapCircleAll(node.transform.position, range);
            foreach(var col in cols){
                GraphNode hitnode;
                if(col.TryGetComponent<GraphNode>(out hitnode) && hitnode != node){
                    //check line of sight
                    var dir = hitnode.transform.position - node.transform.position;
                    var hit = Physics2D.Raycast(node.transform.position + dir.normalized * lookahead, dir);
                    if(hit.collider != null && hit.transform.position == hitnode.transform.position){
                        node.Children.Add(hitnode);

                        //draw edge
                        DrawLine(node.transform.position, hitnode.transform.position, linePrefab, parent);
                    }else{
                        // Debug.Log($"{hit.transform.position} vs. {hitnode.transform.position}");
                    }
                }
            }
        }
        Physics2D.queriesStartInColliders = true;

    }

    public void DeleteNode(GraphNode node, bool isTile){
        //delete from children
        if(isTile){
            foreach(GraphNode neighbor in TileAdjList){
                neighbor.Children.Remove(node);
            }
            TileAdjList.Remove(node);

            node.gameObject.SetActive(false);
        } else {
            foreach(GraphNode neighbor in WaypointAdjList){
                neighbor.Children.Remove(node);
            }
            WaypointAdjList.Remove(node);

            node.gameObject.SetActive(false);
        }

        //delete from adj list
        //destroy

    }

    public List<GraphNode> FindPath(GraphNode startNode, GraphNode targetNode, float HWeight, bool isEuler){
        ResetGH();

        List<GraphNode> toSearch = new List<GraphNode>() {startNode };
        List<GraphNode> processed = new List<GraphNode>();

        startNode.G = 0;
        startNode.H = 0;

        while(toSearch.Count > 0){
            GraphNode current = toSearch[0];

            //does this new node have a better F cost than the current one
            foreach(GraphNode toSearchNode in toSearch){
                if(toSearchNode.F < current.F || toSearchNode.F == current.F && toSearchNode.H < current.H){
                    current = toSearchNode;
                }
            }

            processed.Add(current);
            toSearch.Remove(current);

            if(current == targetNode){
                var currentPathNode = targetNode;
                var path = new List<GraphNode>();
                while(currentPathNode != startNode){
                    path.Add(currentPathNode);
                    currentPathNode = currentPathNode.Connection;
                    currentPathNode.IsPath = true;
                }

                path.Reverse();
                return path;
            }

            //foreach unprocessed neighbor
            foreach(GraphNode neighbor in current.Children.Where(t => !processed.Contains(t))){
                bool inSearch = toSearch.Contains(neighbor);
                float costToNeighbor = current.G + current.GetManhattan(neighbor);

                if(!inSearch || costToNeighbor < neighbor.G){
                    neighbor.G = costToNeighbor;
                    neighbor.Connection = current;

                    if(!inSearch){
                        neighbor.H = (isEuler ? neighbor.GetEuler(targetNode) : neighbor.GetManhattan(targetNode)) * HWeight;
                        toSearch.Add(neighbor);
                    }
                }
            }

        }
        return null;
    }



}
