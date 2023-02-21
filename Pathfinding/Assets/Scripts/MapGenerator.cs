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


    private void Start() {
        GenerateMapFromFile("lak104d");    
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
        // Debug.Log(file.Length);
        // foreach(string row in file){
        //     Debug.Log(row);
        // }
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
        for(int i = 0; i < height; i++){
            // string s = "";
            for(int j = 0; j < width; j++){
                switch(mapFile[i][j]){
                    case '@':
                        var x = Instantiate(outOfBoundsBlock, transform.position.IgnoreZ() + Vector2.down * i + Vector2.right * j, Quaternion.identity);
                        x.transform.parent = this.transform;
                        break;
                    case 'T':
                        var y = Instantiate(treeBlock, transform.position.IgnoreZ() + Vector2.down * i + Vector2.right * j, Quaternion.identity);
                        y.transform.parent = this.transform;
                        break;
                    case '.':
                        var z = Instantiate(walkableBlock, transform.position.IgnoreZ() + Vector2.down * i + Vector2.right * j, Quaternion.identity);
                        z.transform.parent = this.transform;
                        break;
                }
                // s += mapFile[i][j];

            }
            // Debug.Log(s);
        }
      
    }

}
