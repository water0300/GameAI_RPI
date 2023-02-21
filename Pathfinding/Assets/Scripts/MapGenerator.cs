using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using System.Linq;

public class MapGenerator : MonoBehaviour {
    public struct MapProperties {
        public int height;
        public int width;
        public MapProperties(int height, int width) {
            this.height = height;
            this.width = width;
        }
    }
    
    //assumption: these three objects are the same size?
    public GameObject walkableBlock;
    public GameObject treeBlock;
    public GameObject outOfBoundsBlock;
    public float blockSize; 
    public int mapID;
    private List<GameObject> _grid;//todo change to monobehavior of choice
    private static Dictionary<char, GameObject> blockMap;
    private static List<string> mapNames = new List<string>(){
        "lak104d",
        "arena2",
        "AR0011SR",
        "hrt201n"
    };
    private void Start() {
        blockMap = GenerateBlockMap();
        GenerateMapFromFile(mapNames[mapID]);    
    }

    private Dictionary<char, GameObject> GenerateBlockMap(){
        return new Dictionary<char, GameObject>(){
            {'@', outOfBoundsBlock},
            {'T', treeBlock},
            {'.', walkableBlock},

        };
    }

    private bool GenerateMapFromFile(string fileName){
        //find file
        string filePath = Application.streamingAssetsPath + "/Maps/" + fileName + ".map";
        if(!File.Exists(filePath)){
            Debug.LogError($"Filename {fileName} does not exist lmao\n(Path: {filePath})");
            return false;
        } 
        string[] fileLines = File.ReadAllLines(filePath);
        
        //parse header
        string[] fileHeader = fileLines.Take(3).ToArray();
        MapProperties mapProps = HandleMapHeader(fileHeader);

        //deploy map
        string[] file = fileLines.Skip(4).ToArray();
        GenerateMap(file, mapProps.height, mapProps.width);

        return true;
        
    }

    //assumption - map is of right format
    private MapProperties HandleMapHeader(string[] header){
        return new MapProperties(
            Int32.Parse(header[1].Split()[1]),
            Int32.Parse(header[2].Split()[1])
        );

    }

    //-x to +x == left col to right col
    //+y to -y == top row to bot row
    private void GenerateMap(string[] mapFile, int height, int width){
        _grid = new List<GameObject>();
        for(int i = 0; i < height; i++){
            for(int j = 0; j < width; j++){
                var placedBlock = Instantiate(blockMap[mapFile[i][j]], transform.position.IgnoreZ() + Vector2.down * i + Vector2.right * j, Quaternion.identity);
                placedBlock.transform.parent = this.transform;
                _grid.Append(placedBlock);

            }
        }
    }

    private void DestroyMap(){
        foreach(GameObject go in _grid){
            Destroy(go);
        }
        
    }

}

