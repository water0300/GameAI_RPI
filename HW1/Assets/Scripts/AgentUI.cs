using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
public class AgentUI : MonoBehaviour {

    [Header("In Scene References")]
    public AgentCircleIndicator slowdownLineRenderer;
    public AgentCircleIndicator arrivedLineRenderer;
    public AgentCircleIndicator wanderLineRenderer;
    public AgentArrowIndicator accelerationArrow;
    public Agent Agent {get; private set; }

    private GameObject _activeTargetIndicator;

    private void Awake() {
        Agent = GetComponent<Agent>();
    }

    // Update is called once per frame
    void LateUpdate(){
        slowdownLineRenderer.Radius = Agent.slowRadius;
        arrivedLineRenderer.Radius = Agent.targetRadius;
        wanderLineRenderer.Radius = Agent.wanderRadius;
        wanderLineRenderer.Offset = Agent.wanderOffset * Agent.transform.rotation.AsNormVector();
        accelerationArrow.Vector = Agent.Linear;
    }
}
