using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
public class AgentUI : MonoBehaviour {

    [Header("In Scene References")]
    public AgentCircleIndicator slowdownLineRenderer;
    public AgentCircleIndicator arrivedLineRenderer;
    public Agent Agent {get; private set; }

    private GameObject _activeTargetIndicator;

    private void Awake() {
        Agent = GetComponent<Agent>();
    }

    // Update is called once per frame
    void LateUpdate(){
        slowdownLineRenderer.Radius = Agent.slowdownRadius;
        arrivedLineRenderer.Radius = Agent.arrivalRadius;
    }
}
