using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
   
    private MapGenerator _generator;
    Vector3 mouseWorldPos;
    private void Start() {
        _generator = FindObjectOfType<MapGenerator>();
    }
    void Update()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        // Debug.Log(mouseWorldPos);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector3.forward);
        foreach(var hit in hits){
            if(hit.collider.GetComponent<GraphNode>()){
                Debug.Log(hit.collider.name);
            }   
        }
        
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(mouseWorldPos, 1f);
    }
}
