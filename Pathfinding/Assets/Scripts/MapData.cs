using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData {
    
    [field: SerializeField] public List<Block> MapBlockList {get; set; } = new List<Block>();
    [field: SerializeField] public MapDimensions Dimensions {get; set; }


    public Dictionary<GraphNode, List<GraphNode>> AdjList {get; private set; } = new Dictionary<GraphNode, List<GraphNode>>();

    public void GenerateTiles(List<List<Block>> grid, MapDimensions dimensions, int tilesize, GraphNode nodePrefab, Transform nodeParent){

        int rows = Mathf.CeilToInt((float) dimensions.height / (float) tilesize);
        int cols = Mathf.CeilToInt((float) dimensions.width / (float) tilesize);
        GraphNode[,] nodeMatrix = new GraphNode[rows,cols];

        for(int offsetRow = 0; offsetRow < rows; offsetRow += tilesize){
            for(int offsetCol = 0; offsetCol < cols; offsetCol += tilesize){

                //only get the node if walkable
                if(IsWalkable(grid, dimensions, new MapDimensions(offsetRow, offsetCol), tilesize)){
                    //get average position
                    Vector2 nodePos = Vector2.zero;
                    for(int k = offsetRow; k < offsetRow + tilesize && offsetRow + tilesize < dimensions.height; k++){
                        for(int l = offsetCol; l < offsetCol + tilesize && offsetRow + tilesize < dimensions.width; l++){
                            nodePos += grid[k][l].transform.position.IgnoreZ();
                        } 
                    }
                    nodePos /= tilesize * tilesize;

                    GraphNode node = Object.Instantiate(nodePrefab, nodePos, Quaternion.identity);
                    node.transform.parent = nodeParent;
                    nodeMatrix[offsetRow, offsetCol] = node;
                }
            }
        }
    

    }

    //check if all tiles are walkable or not (i.e. if we can reduce tile size)
    private bool IsWalkable(List<List<Block>> grid, MapDimensions dimensions, MapDimensions offset, int tilesize){
        for(int i = offset.height; i < offset.height + tilesize && i + tilesize < dimensions.height; i++){
            for(int j = offset.width; j < offset.width + tilesize && j + tilesize < dimensions.width; j++){
                Debug.Log($"i: {i}, j: {j}");
                if(!grid[i][j].isWalkable){
                    return false;
                }
            }
        }

        return true;
    }

}
