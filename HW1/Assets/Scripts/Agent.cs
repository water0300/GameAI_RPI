using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Agent : MonoBehaviour {
    [Header("Prefab References")]
    public GameObject targetIndicatorPrefab;
    [Header("In Scene References")]
    // public WaypointPool waypointPool;
    public Player player;
    
    [Header("Options")]
    public float maxSpeed;
    public float maxAcceleration = 3f;
    public float maxAngularAccelaration = 3f;
    public float arrivalRadius;
    public float slowdownRadius;
    public float maxPrediction;
    [Range(0.01f, 2f)] public float timeToTarget = 0.1f;

    public Rigidbody TargetRB {get; private set; }
    public Transform Target {get; private set; }
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
        // AgentState = new SeekAndArriveState();
        // AgentState = new FleeState();
        AgentState = new PursueState();


    }

    void FixedUpdate(){
        if(TargetRB != null){
            AgentState.OnUpdate(this, Time.fixedDeltaTime);
            // Debug.Log($"Speed: {Velocity.magnitude}");

        }

    }

    void AssignTarget(Rigidbody newTarget){
        TargetRB = newTarget;
        Target = Instantiate(targetIndicatorPrefab.transform);
    }
    public void SetState(IAgentState state) {
        // if(AgentState != null) yield return StartCoroutine(genericState.OnStateExit());
        AgentState = state;
        // yield return StartCoroutine(genericState.OnStateEnter());
    }
}
