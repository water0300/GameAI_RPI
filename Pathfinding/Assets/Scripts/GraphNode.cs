using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode : MonoBehaviour {
    private SpriteRenderer _renderer;
    [field: SerializeField] public List<GraphNode> Children {get; set; } = new List<GraphNode>();
    [field: SerializeField] public GraphNode Connection {get; set; }
    [field: SerializeField] public float G {get; set; } 
    [field: SerializeField] public float H {get; set; } 
    public bool IsPath = false;
    public float F => G + H;

    void Start(){
        _renderer = GetComponent<SpriteRenderer>();
        G = -1;
        H = -1;
    }

    void Update(){
        if(G == 0 && H == 0){
            _renderer.color = Color.magenta;
        } else if (G == -1 && H == -1){
            _renderer.color = Color.grey;
        } else if (H == 0){
            _renderer.color = Color.cyan;
        } else if(IsPath) {
            _renderer.color = Color.green;
        } else {
            _renderer.color = Color.blue;
        }
    }

    public float GetDistance(GraphNode node){
        return 0f;
    }

    public float GetManhattan(GraphNode node){
        return Mathf.Abs(transform.position.x - node.transform.position.x) + Mathf.Abs(transform.position.y - node.transform.position.y);

    }


}
