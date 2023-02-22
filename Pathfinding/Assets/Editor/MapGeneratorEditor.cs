using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {
    public MapGenerator MapGenerator {get; private set; }
    private int _selected = 0;
    private string[] _options = new string[]{"lak104d", "arena2", "AR0011SR", "hrt201n"};
    private void OnEnable() {
        MapGenerator = (MapGenerator) target;
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        EditorGUILayout.LabelField("Map Generation", EditorStyles.boldLabel);

        _selected = EditorGUILayout.Popup("Choose Map", _selected, _options); 
        string filename = _options[_selected];
        if(GUILayout.Button($"Generate {filename}")){
            OnMapGenerate(filename);
        }

        if(MapGenerator.MapData != null && MapGenerator.MapData.MapBlocks.Count > 0 && GUILayout.Button($"Clear {filename}")){
            OnMapClear(); 
        } else if(MapGenerator.MapData != null && GUILayout.Button($"asdfasdf { MapGenerator.MapData.MapBlocks.Count}")){

        }
    }

    private void OnMapGenerate(string filename){
        MapGenerator.ClearMap();
        MapGenerator.GenerateMapFromFile(filename); //todo coupling UI text to logic a nono?
    } 
    
    private void OnMapClear() => MapGenerator.ClearMap();

}

