using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData {
    
    [field: SerializeField] public List<Block> MapBlockList {get; set; } = new List<Block>();
    [field: SerializeField] public MapDimensions Dimensions {get; set; }

    public List<GraphNode> AdjList = new List<GraphNode>();

    public Block MapBlockGet(int row, int col){
        return MapBlockList[row * Dimensions.width + col];
    }

    public void GenerateTiles(int tilesize, GraphNode nodePrefab, Transform nodeParent){

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
        SetupAdjList(nodeMatrix);

    }

    private static (int, int)[] directions = new (int, int)[]{
        (-1,-1),
        (-1,0),
        (-1,1),
        (0,-1),
        (0,1),
        (1,-1),
        (1,0),
        (1,1)
    };

    
    public List<(Vector3, Vector3)> edges = new List<(Vector3, Vector3)>();
    private void SetupAdjList(GraphNode[,] nodeMatrix){
        for(int i = 0; i < nodeMatrix.GetLength(0); i++){
            for(int j = 0; j < nodeMatrix.GetLength(1); j++){
                //check all 8 directions
                if(nodeMatrix[i,j] == null){
                    continue;
                }

                AdjList.Add(nodeMatrix[i,j]);

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
                    //temporarily draw list
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



    public bool AStar(){
        return true;
    }

}
