using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;

public class MapGenerator : MonoBehaviour {
    
    [Header("In Scene Refs")]
    public Transform nodeParent;

    [Header("Prefabs")]
    public Block walkableBlock;
    public Block treeBlock;
    public Block outOfBoundsBlock;
    public GraphNode graphNodePrefab;


    [Header("Props")]
    public float blockSize = 1f; 
    public int tileSize = 3;
    private Dictionary<char, Block> _blockMap;

    [field: SerializeField] public MapDimensions Dimensions {get; set; }
    [field: SerializeField] public List<List<Block>> MapBlockGrid {get; private set; } = new List<List<Block>>();
    [field: SerializeField] public MapData MapData {get; private set; } 

    private void GenerateBlockPrefabMap(){
        _blockMap = new Dictionary<char, Block>(){
            {'@', outOfBoundsBlock},
            {'T', treeBlock},
            {'.', walkableBlock}, 

        };
    }

    public bool GenerateMapFromFile(string fileName){
        //find file
        string filePath = Application.streamingAssetsPath + "/Maps/" + fileName + ".map";
        if(!File.Exists(filePath)){
            Debug.LogError($"Filename {fileName} does not exist lmao\n(Path: {filePath})");
            return false;
        } 
        string[] fileLines = File.ReadAllLines(filePath);

        //parse header
        string[] fileHeader = fileLines.Take(3).ToArray();
        Dimensions = HandleMapHeader(fileHeader);

        //deploy map
        string[] file = fileLines.Skip(4).ToArray();
        GenerateMap(file);

        return true;
        
    }

    //assumption - map is of right format
    private MapDimensions HandleMapHeader(string[] header){
        return new MapDimensions(
            Int32.Parse(header[1].Split()[1]),
            Int32.Parse(header[2].Split()[1])
        );

    }

    //-x to +x == left col to right col
    //+y to -y == top row to bot row
    private void GenerateMap(string[] mapFile){
        if(_blockMap == null){
            GenerateBlockPrefabMap();
        }
        for(int i = 0; i < Dimensions.height; i++){
            MapBlockGrid.Add(new List<Block>());
            for(int j = 0; j < Dimensions.width; j++){
                Block placedBlock = Instantiate(_blockMap[mapFile[i][j]], transform.position.IgnoreZ() + Vector2.down * i * blockSize + Vector2.right * j  * blockSize, Quaternion.identity);
                placedBlock.transform.parent = this.transform; 
                MapBlockGrid[i].Add(placedBlock);

            }
        }
    }

    private void Start() {
        MapData = new MapData();
        // Debug.Log(MapBlockGrid == null);
        MapData.GenerateTiles(MapBlockGrid, Dimensions, tileSize, graphNodePrefab, nodeParent);
    }

    public void ClearMap(){
        if(MapBlockGrid.Count == 0){
            Debug.Log("grid was null (DELETION)");
            return;
        } else {
            foreach(var goList in MapBlockGrid){
                foreach(Block go in goList){
                    DestroyImmediate(go.gameObject);
                }
            }
            MapBlockGrid.Clear(); 
        }
        
    }

    public void PurgeMap(){
        while(transform.childCount > 0){
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

}

