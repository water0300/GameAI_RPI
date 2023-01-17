using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Agent : MonoBehaviour {
    [Header("In Scene References")]
    // public WaypointPool waypointPool;
    public AgentManager player;
    
    [Header("Options")]
    public float maxSpeed;
    public float accelerationFactor;
    public float arrivalRadius;
    public float slowdownRadius;

    public GameObject Target {get; private set; }
    public IAgentState AgentState {get; private set; }
    public Rigidbody Rb {get; private set; }
    public Vector3 Velocity {get; set; } = Vector3.zero; 
    public Quaternion AngularVelocity {get; set; } = Quaternion.identity; 

    private void OnEnable() {
        // waypointPool.OnWaypointSelect += AssignTarget;
        player.OnTargetActivate += AssignTarget;
    }
    private void OnDisable() {
        // waypointPool.OnWaypointSelect -= AssignTarget;
        player.OnTargetActivate -= AssignTarget;
    }
    void Awake(){
        Rb = GetComponent<Rigidbody>();
    }

    void Start(){
        AgentState = new SeekState();
        // AgentState = new FleeState();


    }

    void FixedUpdate(){
        if(Target != null){
            AgentState.OnUpdate(this, AgentState.GetSteering(this), Time.fixedDeltaTime);
        }

    }

    void AssignTarget(GameObject newTarget){
        Target = newTarget;
    }
    public void SetState(IAgentState state) {
        // if(AgentState != null) yield return StartCoroutine(genericState.OnStateExit());
        AgentState = state;
        // yield return StartCoroutine(genericState.OnStateEnter());
    }
}
