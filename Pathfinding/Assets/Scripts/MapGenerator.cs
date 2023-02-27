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
    public int tileSize = 2;
    private Dictionary<char, Block> _blockMap;

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

        MapData = new MapData();

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
        if(_blockMap == null){
            GenerateBlockPrefabMap();
        }
        for(int i = 0; i < MapData.Dimensions.height; i++){
            for(int j = 0; j < MapData.Dimensions.width; j++){
                Block placedBlock = Instantiate(_blockMap[mapFile[i][j]], transform.position.IgnoreZ() + Vector2.down * i * blockSize + Vector2.right * j  * blockSize, Quaternion.identity);
                placedBlock.transform.parent = this.transform; 
                MapData.MapBlockList.Add(placedBlock);

            }
        } 
    }

    private void Start() {
        MapData.GenerateTiles(tileSize, graphNodePrefab, nodeParent);
        // Debug.Log(MapBlockGrid == null);
        // MapData.GenerateTiles(MapBlockGrid, Dimensions, tileSize, graphNodePrefab, nodeParent);
    }

    private void Update() {

    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        if(MapData != null){
            foreach(var edge in MapData.edges){
                Gizmos.DrawLine(edge.Item1, edge.Item2);
            }
        }

    }

    public void ClearMap(){
        if(MapData.MapBlockList == null){
            Debug.Log("mapdata grid was null, no deletion neccesary");
            return;
        } else {
            foreach(Block go in MapData.MapBlockList){
                DestroyImmediate(go.gameObject);
            }
            MapData.MapBlockList.Clear(); 
        }
        
    }

    public void PurgeMapData(){
        while(transform.childCount > 0){
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        MapData = null;
        Debug.Log("Purged!");
    }

}

