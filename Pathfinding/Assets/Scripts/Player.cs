using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Agent _agent;
    private MapGenerator _generator;
    Vector3 mouseWorldPos;

    [Range(0.01f, 1f)] public float HWeight = 1f; //todo: DONT ALLOW == 0
    public bool isEuler = false;

    public GraphNode currStartNode = null;
    public GraphNode currEndNode = null;

    bool startOrEndPlace = true; //true == start, false == end;
    

    private void Start() {
        _generator = FindObjectOfType<MapGenerator>();
        _agent = FindObjectOfType<Agent>();
    }
    void Update(){
        if(Input.GetMouseButtonDown(0)){
            if(startOrEndPlace){
                currStartNode = GetNodeAtMousePos();
                if(currStartNode != null){
                    currStartNode.H = 0;
                    currStartNode.G = 0;
                }
            } else {
                currEndNode = GetNodeAtMousePos();
                if(currEndNode != null){
                    currEndNode.H = 0;
                }
            }
            startOrEndPlace = !startOrEndPlace;

        }
        
        if(Input.GetMouseButtonDown(1)){
            GraphNode node = GetNodeAtMousePos();
            if(node != null){
                _generator.MapData.DeleteNode(node);
            }
        }

        if(currStartNode != null && currEndNode != null){
            _generator.MapData.FindPath(currStartNode, currEndNode, HWeight, isEuler);
            // _agent.Steer.currNode = 0;
        }


    }

    private GraphNode GetNodeAtMousePos(){
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
