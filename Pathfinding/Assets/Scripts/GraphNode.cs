using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GraphNode : MonoBehaviour {
    private SpriteRenderer _renderer;
    public TMP_Text text;
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
        if(G == 0){
            text.text = $"Start";
            _renderer.color = Color.magenta;
        } else if (G == -1 && H == -1){
            text.text = $"";
            _renderer.color = Color.grey;
        } else if (H == 0){
            text.text = $"End";
            _renderer.color = Color.magenta;
        } else if(IsPath) {
            text.text = $"G={Math.Round(G, 2)}\nH:{Math.Round(H, 2)}";

            _renderer.color = Color.green;
        } else {
            text.text = $"G={Math.Round(G, 2)}\nH:{Math.Round(H, 2)}";

            _renderer.color = Color.cyan;
        }

    }


    public float GetManhattan(GraphNode node){
        return Mathf.Abs(transform.position.x - node.transform.position.x) + Mathf.Abs(transform.position.y - node.transform.position.y);
    }

    public float GetEuler(GraphNode node){
        return Vector3.Distance(transform.position, node.transform.position);
    }

    private void OnDrawGizmos() {
        foreach(var child in Children){
            Gizmos.DrawLine(transform.position, child.transform.position);
        }
    }


}
