using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
public class AgentUI : MonoBehaviour {
    [Header("In Scene References")]
    public AgentCircleIndicator slowdownUI;
    public AgentCircleIndicator arrivedUI;
    public Agent Agent {get; private set; }

    private void Awake() {
        Agent = GetComponent<Agent>();
    }

    // Update is called once per frame
    void LateUpdate(){
        slowdownUI.Radius = Agent.slowdownRadius;
        arrivedUI.Radius = Agent.arrivalRadius;
    }
}
