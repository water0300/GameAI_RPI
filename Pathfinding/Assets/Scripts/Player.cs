using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
   private Agent _agent;
    private MapGenerator _generator;
    Vector3 mouseWorldPos;

    public GraphNode currStartNode = null;
    public GraphNode currEndNode = null;

    private void Start() {
        _generator = FindObjectOfType<MapGenerator>();
        _agent = FindObjectOfType<Agent>();
    }
    void Update(){
        GraphNode startNode = GetStartNode();
        GraphNode endNode = GetEndNode();

        if(startNode == null || endNode == null){
            return;
        }

        if(startNode != currStartNode || endNode != currEndNode){
            Debug.Log(startNode.name);
            Debug.Log(endNode.name);
            _generator.MapData.FindPath(startNode, endNode);
            currStartNode = startNode;
            currEndNode = endNode;
        }
    }

    private GraphNode GetStartNode(){
        if(_generator.MapData.AdjList.Count > 0){
            GraphNode closest = null;
            float shortestDist = Mathf.Infinity;
            foreach(GraphNode node in _generator.MapData.AdjList){
                float currDist = Vector3.Distance(node.transform.position, _agent.transform.position);
                if(currDist < shortestDist){
                    shortestDist = currDist;
                    closest = node;
                }
            }
            return closest;
        } else {
            return null;
        }

    }

    private GraphNode GetEndNode(){
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector3.forward);
        foreach(var hit in hits){
            GraphNode node;
            if(hit.collider.TryGetComponent<GraphNode>(out node)){
                return node;
            }   
        }

        return null;
    }

    private void OnDrawGizmos() {
        // Gizmos.DrawSphere(mouseWorldPos, .2f);
    }
}
