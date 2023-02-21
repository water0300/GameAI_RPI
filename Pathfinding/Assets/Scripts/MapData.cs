using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : ScriptableObject{
    public List<GameObject> MapBlocks {get; private set; } = new List<GameObject>();

    public void AddMapBlock(GameObject block) => MapBlocks.Add(block);


    #if UNITY_EDITOR
    public void DestroyMapBlocks(){
        foreach(GameObject go in MapBlocks){
            // go.SetActive(false);
            Object.DestroyImmediate(go);
            // Destroy(go);
        }
        MapBlocks.Clear();
    }
    #endif
}
