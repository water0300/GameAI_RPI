using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public event Action<Vector3> OnWaypointSpawn;
    public event Action<GameObject> OnWaypointDragToggle;

    public void IssueCommand(Vector2 mousePos){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)){
            OnWaypointSpawn?.Invoke(hit.point);
        }
    }



}
