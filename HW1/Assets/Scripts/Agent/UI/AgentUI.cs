using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Agent))]
public class AgentUI : MonoBehaviour {

    [Header("In Scene References")]
    public AgentCircleIndicator slowdownLineRenderer;
    public AgentCircleIndicator arrivedLineRenderer;
    public AgentCircleIndicator wanderLineRenderer;
    public AgentArrowIndicator accelerationArrow;
    public GameObject pathFollowUI;
    public TMP_Text infotext;
    public Agent Agent {get; private set; }


    private void OnEnable() {
        Agent.OnStateChange += OnStateChange;
    }
    private void OnDisable() {
        Agent.OnStateChange -= OnStateChange;
    }
    private void Awake() {
        Agent = GetComponent<Agent>();
    }


    void LateUpdate(){
        switch(Agent.ActiveState){
            case PursueState _:
                slowdownLineRenderer.Radius = Agent.SlowRadius;
                arrivedLineRenderer.Radius = Agent.TargetRadius;
                break;
            case FleeState _:
                break;
            case WanderState _:
                wanderLineRenderer.Radius = Agent.WanderRadius;
                wanderLineRenderer.Offset = Agent.WanderOffset * Agent.transform.rotation.AsNormVector();
                break;
            case FollowPathState:
                break;
            default:
                break;

        }

        accelerationArrow.Vector = Agent.Linear;
        infotext.text = Agent.statusText;
    }

    void OnStateChange(){ //todo make less shitty
        switch(Agent.ActiveState){
            case PursueState _:
                slowdownLineRenderer.gameObject.SetActive(true);
                arrivedLineRenderer.gameObject.SetActive(true);
                wanderLineRenderer.gameObject.SetActive(false);
                pathFollowUI.gameObject.SetActive(false);

                break;
            case FleeState _:
                slowdownLineRenderer.gameObject.SetActive(false);
                arrivedLineRenderer.gameObject.SetActive(false);
                wanderLineRenderer.gameObject.SetActive(false);
                pathFollowUI.gameObject.SetActive(false);
                break;
            case WanderState _:
                slowdownLineRenderer.gameObject.SetActive(false);
                arrivedLineRenderer.gameObject.SetActive(false);
                wanderLineRenderer.gameObject.SetActive(true);
                pathFollowUI.gameObject.SetActive(false);

                break;
            case FollowPathState:
                slowdownLineRenderer.gameObject.SetActive(false);
                arrivedLineRenderer.gameObject.SetActive(false);
                wanderLineRenderer.gameObject.SetActive(false);
                pathFollowUI.gameObject.SetActive(true);
                break;
            default:
                break;

        }

    }



}
