using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Agent))]
public class AgentUI : MonoBehaviour {

    [Header("In Scene References")]
    // public AgentCircleIndicator slowdownLineRenderer;
    // public AgentCircleIndicator arrivedLineRenderer;
    // public AgentCircleIndicator wanderLineRenderer;
    public AgentCircleIndicator thresholdRenderer;
    public AgentArrowIndicator coneCheck1;
    public AgentArrowIndicator coneCheck2;
    public AgentArrowIndicator raycastCheck;


    public AgentArrowIndicator accelerationArrow;
    // public GameObject pathFollowUI;
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
    private void Start() {
        thresholdRenderer.Radius = Agent.Threshold;
    }


    void LateUpdate(){
        
        switch(Agent.index){
            case 0:
                float angle = Mathf.Acos(Agent.ConeThreshold);
                Vector3 angleA = DirFromAngle(angle/2 + (Agent.transform.eulerAngles.y - 90f) * Mathf.Deg2Rad);
                Vector3 angleB = DirFromAngle(-angle/2 + (Agent.transform.eulerAngles.y - 90f) * Mathf.Deg2Rad);
                coneCheck1.Vector = (angleA * Agent.Threshold);
                coneCheck2.Vector = (angleB * Agent.Threshold);

                break;
            case 1:
                raycastCheck.Vector = (DirFromAngle((Agent.transform.eulerAngles.y - 90f) * Mathf.Deg2Rad) * Agent.Threshold); 

                break;
        }
        accelerationArrow.Vector = Agent.Linear;
        // infotext.text = Agent.statusText;
    }
    private Vector3 DirFromAngle(float angleInRad) => new Vector3(Mathf.Sin(angleInRad), 0, Mathf.Cos(angleInRad));

    void OnStateChange(){ 
        switch(Agent.index){
            case 0:
                coneCheck1.gameObject.SetActive(true);
                coneCheck2.gameObject.SetActive(true);
                raycastCheck.gameObject.SetActive(false);
                break;
            case 1:
                coneCheck1.gameObject.SetActive(false);
                coneCheck2.gameObject.SetActive(false);
                raycastCheck.gameObject.SetActive(true);

                break;
            case 2:
                coneCheck1.gameObject.SetActive(false);
                coneCheck2.gameObject.SetActive(false);
                raycastCheck.gameObject.SetActive(false);
                break;
        }
        
        //todo make less shitty
        // switch(Agent.ActiveState){
        //     case PursueSubState _:
        //         slowdownLineRenderer.gameObject.SetActive(true);
        //         arrivedLineRenderer.gameObject.SetActive(true);
        //         wanderLineRenderer.gameObject.SetActive(false);
        //         // pathFollowUI.gameObject.SetActive(false);

        //         break;
        //     case FleeSubState _:
        //         slowdownLineRenderer.gameObject.SetActive(false);
        //         arrivedLineRenderer.gameObject.SetActive(false);
        //         wanderLineRenderer.gameObject.SetActive(false);
        //         // pathFollowUI.gameObject.SetActive(false);
        //         break;
        //     case WanderSubState _:
        //         slowdownLineRenderer.gameObject.SetActive(false);
        //         arrivedLineRenderer.gameObject.SetActive(false);
        //         wanderLineRenderer.gameObject.SetActive(true);
        //         // pathFollowUI.gameObject.SetActive(false);

        //         break;
        //     case FollowPathSubState:
        //         slowdownLineRenderer.gameObject.SetActive(false);
        //         arrivedLineRenderer.gameObject.SetActive(false);
        //         wanderLineRenderer.gameObject.SetActive(false);
        //         // pathFollowUI.gameObject.SetActive(true);
        //         break;
        //     default:
        //         break;

        // }

    }



}
