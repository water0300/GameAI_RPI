using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    public GameObject targetPrefab;
    public event Action<Vector3> OnWaypointSpawn;
    public event Action<GameObject> OnTargetActivate;
    // public event Action<Vector3> OnTargetMove;
    private GameObject _activePlayerTarget;

    private void Start() {
        _activePlayerTarget = Instantiate(targetPrefab);
        OnTargetActivate?.Invoke(_activePlayerTarget);
    }
    public void OnMouseMoved(Vector2 mousePos){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Debug.Log("asdf");

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("World"))){
            // Debug.Log(hit.transform.gameObject.layer);
            _activePlayerTarget.transform.position = hit.point;
            // OnWaypointSpawn?.Invoke(hit.point);
        }
    }

    // public void IssueCommand(){
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     RaycastHit hit;
    //     if(Physics.Raycast(ray, out hit, Mathf.Infinity, ~6)){
    //         Debug.Log(hit.transform.gameObject.layer);
    //         OnWaypointSpawn?.Invoke(hit.point);
    //     }

    // }



}
