using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(StatData))]
public class StatDataEditor : Editor {
    public StatData StatData {get; private set; }
    private void OnEnable() {
        StatData = (StatData) target;
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        if(GUILayout.Button($"Save to TXT")){
            SaveToTXT();
        }
    }

    private void SaveToTXT(){
        string _overwriteFile = Application.dataPath + "/Python/data.txt";
        using(var writer = new StreamWriter(_overwriteFile, false)){
            foreach(var data in StatData.data){
                writer.WriteLine($"{data.timestep}, {data.populationCount}");
            }
        }

    }  
    

}

