using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData {
    
    [field: SerializeField] public List<Block> MapBlockList {get; set; } = new List<Block>();
    [field: SerializeField] public MapDimensions Dimensions {get; set; }


    public Dictionary<GraphNode, List<GraphNode>> AdjList {get; private set; } = new Dictionary<GraphNode, List<GraphNode>>();

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
                Debug.Log($"Curr Offset: {offsetRow}, {offsetCol}, < {Dimensions.height}, {Dimensions.width}");

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


    }

    //check if all tiles are walkable or not (i.e. if we can reduce tile size)
    private bool IsWalkable(int offsetRow, int offsetCol, int tilesize){
        // Debug.Log($"Curr Offset: {rowOffset}, {colOffset}, con: {rowOffset + tilesize}, {colOffset + tilesize}");

        for(int i = offsetRow; i < Dimensions.height && i - offsetRow < tilesize; i++){
            for(int j = offsetCol; j < Dimensions.width && j - offsetCol < tilesize; j++){
                Debug.Log($"Check Offset: {i}, {j}, < {Dimensions.height}, {Dimensions.height}");
                if(!MapBlockGet(i, j).isWalkable){
                    return false;
                }
            }
        }

        return true;
    }

}
