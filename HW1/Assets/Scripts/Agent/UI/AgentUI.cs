using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Agent))]
public class AgentUI : MonoBehaviour {

    [Header("In Scene References")]
    public AgentCircleIndicator slowdownLineRenderer;
    public AgentCircleIndicator arrivedLineRenderer;
    public AgentCircleIndicator wanderLineRenderer;
    public AgentArrowIndicator accelerationArrow;
    public TMP_Text infotext;
    public Agent Agent {get; private set; }

    private GameObject _activeTargetIndicator;

    private void OnEnable() {
        Agent.OnStateChange += OnStateChange;
    }
    private void OnDisable() {
        Agent.OnStateChange -= OnStateChange;
    }
    private void Awake() {
        Agent = GetComponent<Agent>();
    }

    private void UpdateInfotext(){ //todo idk if this should be update loop
        infotext.text = Agent.statusText;
    }

    // Update is called once per frame
    void LateUpdate(){
        switch(Agent.ActiveState){
            case PursueState _:
                slowdownLineRenderer.Radius = Agent.slowRadius;
                arrivedLineRenderer.Radius = Agent.targetRadius;
                break;
            case FleeState _:
                break;
            case WanderState _:
                wanderLineRenderer.Radius = Agent.wanderRadius;
                wanderLineRenderer.Offset = Agent.wanderOffset * Agent.transform.rotation.AsNormVector();
                break;
            case FollowPathState:
                break;
            default:
                break;

        }

        accelerationArrow.Vector = Agent.Linear;
        UpdateInfotext();
    }

    void OnStateChange(){ //todo make less shitty
        switch(Agent.ActiveState){
            case PursueState _:
                slowdownLineRenderer.gameObject.SetActive(true);
                arrivedLineRenderer.gameObject.SetActive(true);
                wanderLineRenderer.gameObject.SetActive(false);
                break;
            case FleeState _:
                slowdownLineRenderer.gameObject.SetActive(false);
                arrivedLineRenderer.gameObject.SetActive(false);
                wanderLineRenderer.gameObject.SetActive(false);
                break;
            case WanderState _:
                slowdownLineRenderer.gameObject.SetActive(false);
                arrivedLineRenderer.gameObject.SetActive(false);
                wanderLineRenderer.gameObject.SetActive(true);
                break;
            case FollowPathState:
                slowdownLineRenderer.gameObject.SetActive(false);
                arrivedLineRenderer.gameObject.SetActive(false);
                wanderLineRenderer.gameObject.SetActive(false);
                break;
            default:
                break;

        }
    }
}
