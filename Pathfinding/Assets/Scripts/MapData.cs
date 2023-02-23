using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData {
    [field: SerializeField] public MapDimensions Dimensions {get; set; }
    [field: SerializeField] public List<GameObject> MapBlocks {get; private set; } = new List<GameObject>();

    public void AddMapBlock(GameObject block) => MapBlocks.Add(block);


    public void ClearBlocks() => MapBlocks.Clear();
    
}
