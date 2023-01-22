using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PathHandler : MonoBehaviour {

    [Header("Prefab References")]
    public GameObject nodePrefab;

    [Header("Options")]
    public float pathSegmentLength = 3f;
    public event Action<Path> OnPathUpdate;

    public bool IsCreatingPath {get; set; } = true;
    public Vector2 MouseAxis {get; set; }
    private int _nodeCount = 0;
    private GameObject _currNode;
    private Path _currPath;
    private bool _pointerOverUI = false;
    public void SpawnPathNode(){
        if(IsCreatingPath && !_pointerOverUI){
            Ray ray = Camera.main.ScreenPointToRay(MouseAxis);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("World"))){
                if(_nodeCount != 0){
                    _currPath.Segments.Add(new PathSegment(_currNode.transform.position, GetNodePos(hit.point)));
                    _currNode = Instantiate(nodePrefab, GetNodePos(hit.point), Quaternion.identity);
                } else {
                    _currPath = new Path();
                    _currNode = Instantiate(nodePrefab, hit.point, Quaternion.identity);
                }
                _nodeCount++;
            }
        }
    }

    private Vector3 GetNodePos(Vector3 point) => _currNode.transform.position + (point - _currNode.transform.position).normalized * pathSegmentLength;

    public void FinalizePath(){
        OnPathUpdate?.Invoke(_currPath);

    }

    public void ClearPath() {
        _currNode = null;
        _nodeCount = 0;
        _currPath = null;
        OnPathUpdate?.Invoke(null);
    }

    //todo: ui should be in dedicated script? 
    public void Update(){
        _pointerOverUI = EventSystem.current.IsPointerOverGameObject();
    }
    
    private void OnDrawGizmos() {
        if(_currNode != null){
            Ray ray = Camera.main.ScreenPointToRay(MouseAxis);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("World"))){
                // Debug.Log(hit.point);
                Gizmos.DrawLine(_currNode.transform.position, GetNodePos(hit.point));
            }
        }
    }

    


}
