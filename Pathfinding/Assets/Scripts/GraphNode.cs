using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode : MonoBehaviour {
    [field: SerializeField] public List<Block> AssignedBlocks {get; set; } 
}
