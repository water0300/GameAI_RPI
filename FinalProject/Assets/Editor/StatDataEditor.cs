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
        if(GUILayout.Button($"Save to TXT")){
            SaveToTXT();
        }

        base.OnInspectorGUI();

    }

    private void SaveToTXT(){
        WriteHelper("/Python/herbivore_data.txt", StatData.herbivoreData);
        WriteHelper("/Python/carnivore_data.txt", StatData.carnivoreData);

    }  

    private void WriteHelper(string filepath, List<StatDataObj> datalist){
        string file = Application.dataPath + filepath;
        using(var writer = new StreamWriter(file, false)){
            foreach(var data in datalist){
                writer.WriteLine(
                    $"{data.timestep}, {data.populationCount}, {data.avgMaxSpeed}, {data.averageForwardBias}, {data.avgMetabolism}, {data.avgDetectionRadius}, {data.avgDesire}, {data.avgGestationDuration}");
            }
        }
    }
    

}

