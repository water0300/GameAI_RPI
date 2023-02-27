using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Player : MonoBehaviour
{
    private Agent _agent;
    private MapGenerator _generator;
    Vector3 mouseWorldPos;

    [Range(0.01f, 1f)] public float HWeight = 1f; //todo: DONT ALLOW == 0
    public bool isEuler = false;
    public bool isTile = true;

    [Header("UI stuff")]
    public TMP_Dropdown mapDropdown;
    public TMP_Dropdown tileTypeDropdown;
    public Slider weightSlider;
    public TMP_Dropdown heuristicDropdown;

    // public void OnMapDelete() => _mapGenerator.ClearMap();


    public GraphNode currStartNode = null;
    public GraphNode currEndNode = null;

    bool startOrEndPlace = true; //true == start, false == end;
    bool _pointerOverUI = false;

    private void Start() {
        _generator = FindObjectOfType<MapGenerator>();
        _agent = FindObjectOfType<Agent>();
    }

    public void OnMapChange(){

    }

    public void OnTileTypeChange(){
        if(tileTypeDropdown.value == 0){
            isTile = true;
        } else {
            isTile = false;
        }
        _generator.tileParent.gameObject.SetActive(isTile);
        _generator.waypointParent.gameObject.SetActive(!isTile);
        isTile = !isTile;
        currStartNode = null;
        currEndNode = null;
        _generator.MapData.ResetGH();
    }

    public void OnHeuristicWeightChange(){
        HWeight = weightSlider.value;

        
        if(currStartNode != null && currEndNode != null){
                    // Debug.Log("wot");

            _generator.MapData.FindPath(currStartNode, currEndNode, HWeight, isEuler);
        }
    }

    public void OnHeuristicChange(){
        if(heuristicDropdown.value == 0){
            isEuler = false;
        } else {
            isEuler = true;
        }

        if(currStartNode != null && currEndNode != null){
            _generator.MapData.FindPath(currStartNode, currEndNode, HWeight, isEuler);
        }

    }

    public void OnReset(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Update(){
        _pointerOverUI = EventSystem.current.IsPointerOverGameObject();
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector3.forward);
        foreach(var hit in hits){
            GraphNode node;
            if(hit.collider.TryGetComponent<GraphNode>(out node)){
                _pointerOverUI = false;
            }   
        }



        if(Input.GetMouseButtonDown(0) && !_pointerOverUI){
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
        
        if(Input.GetMouseButtonDown(1) && !_pointerOverUI){
            GraphNode node = GetNodeAtMousePos();
            if(node != null){
                Debug.Log("asdf");
                _generator.MapData.DeleteNode(node, isTile);
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



}