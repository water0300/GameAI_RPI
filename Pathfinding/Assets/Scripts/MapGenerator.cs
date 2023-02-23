using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;

public class MapGenerator : MonoBehaviour {
    
    //assumption: these three objects are the same size?
    [Header("Prefabs")]
    public GameObject walkableBlock;
    public GameObject treeBlock;
    public GameObject outOfBoundsBlock;

    [Header("Props")]
    public float blockSize = 1f; 
    [field: SerializeField] public MapData MapData {get; private set; } 
    private Dictionary<char, GameObject> blockMap;

    

    private void GenerateBlockPrefabMap(){
        blockMap = new Dictionary<char, GameObject>(){
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
        
        //fill in data
        if(MapData == null){
            Debug.Log("grid was null (GENERATE MAP)");
            MapData = new MapData(); 
        }

        //parse header
        string[] fileHeader = fileLines.Take(3).ToArray();
        MapData.Dimensions = HandleMapHeader(fileHeader);

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
        if(blockMap == null){
            GenerateBlockPrefabMap();
        }
        for(int i = 0; i < MapData.Dimensions.height; i++){
            for(int j = 0; j < MapData.Dimensions.width; j++){
                GameObject placedBlock = Instantiate(blockMap[mapFile[i][j]], transform.position.IgnoreZ() + Vector2.down * i * blockSize + Vector2.right * j  * blockSize, Quaternion.identity);
                placedBlock.transform.parent = this.transform; 
                MapData.AddMapBlock(placedBlock);

            }
        }
    }

    private void Start() {
        Debug.Log(MapData == null);
    }

    public void ClearMap(){
        if(MapData == null){
            Debug.Log("grid was null (DELETION)");
            return;
        } else {
            foreach(GameObject go in MapData.MapBlocks){
                DestroyImmediate(go);
            }
            MapData.ClearBlocks(); 
        }
        
    }

}

