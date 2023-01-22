using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PathHandler : MonoBehaviour {


    [Header("Prefab References")]
    public GameObject nodePrefab;
    public LineRenderer linePrefab;

    [Header("Options")]
    public float pathSegmentLength = 3f;
    public event Action<Path> OnPathUpdate;

    public bool IsCreatingPath {get; set; } = true;
    public Vector2 MouseAxis {get; set; }
    private int _nodeCount = 0;
    private GameObject _currNode;
    private LineRenderer _currNodeLine;
    private Path _currPath;
    private List<LineRenderer> _visiblePath = new List<LineRenderer>(); //todo combine this with the actual path object?
    private List<GameObject> _visiblePathNodes = new List<GameObject>();
    private bool _pointerOverUI = false;
    public void SpawnPathNode(){
        if(IsCreatingPath && !_pointerOverUI && enabled){
            Ray ray = Camera.main.ScreenPointToRay(MouseAxis);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("World"))){
                if(_nodeCount != 0){
                    PathSegmentData data = new PathSegmentData(_currNode.transform.position, GetNodePos(hit.point));
                    _currPath.Segments.Add(data);
                    _currNode = Instantiate(nodePrefab, GetNodePos(hit.point), Quaternion.identity);
                } else {
                    _currPath = new Path();
                    _currNode = Instantiate(nodePrefab, hit.point, Quaternion.identity);
                }
                _currNodeLine = Instantiate(linePrefab);
                _visiblePath.Add(_currNodeLine);
                _visiblePathNodes.Add(_currNode);
                _nodeCount++;
            }
        }
    }

    private Vector3 GetNodePos(Vector3 point) => _currNode.transform.position + (point - _currNode.transform.position).normalized * pathSegmentLength;

    public void FinalizePath(){
        //todo ugly but neccesary?
        _visiblePath.Remove(_currNodeLine);
        Destroy(_currNodeLine.gameObject);
        _currNodeLine = null;

        OnPathUpdate?.Invoke(_currPath);

    }

    public void ClearPath() {
        _currNode = null;
        _currNodeLine = null;
        _nodeCount = 0;
        _currPath = null;
        foreach(LineRenderer lr in _visiblePath){
            lr.gameObject.SetActive(false);
        }
        _visiblePath.Clear();
        foreach(GameObject go in _visiblePathNodes){
            go.SetActive(false);
        }
        _visiblePathNodes.Clear();
        OnPathUpdate?.Invoke(null);
    }

    //todo: ui should be in dedicated script? 
    private void Update(){
        _pointerOverUI = EventSystem.current.IsPointerOverGameObject();
    }

    private void LateUpdate(){
        if(_currNodeLine != null){
            Ray ray = Camera.main.ScreenPointToRay(MouseAxis);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("World"))){
                // Debug.Log(hit.point);
                _currNodeLine.SetPosition(0, _currNode.transform.position);
                _currNodeLine.SetPosition(1, GetNodePos(hit.point));
            }
        }
    
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
