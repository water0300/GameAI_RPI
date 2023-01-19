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
    
    [Header("Position Modifiers")]
    public float maxSpeed;
    public float maxAcceleration = 3f;
    public float targetRadius;
    public float slowRadius;
    public float maxPredictionLookahead;
    [Range(0.01f, 2f)] public float timeToTarget = 0.1f;

    [Header("Rotation Modifiers")]
    public float maxAngularSpeed_Y = 10f;
    public float maxAngularAcceleration_Y = 3f;
    [Range(0f, 180f)]public float targetAlignWindow;
    [Range(0f, 180f)]public float slowAlignWindow;
    [Range(0.01f, 2f)] public float timeToAlign = 0.1f;

    public Rigidbody TargetRB {get; private set; }
    public Transform Target {get; private set; }
    public AgentState AgentState {get; private set; }
    public Rigidbody Rb {get; private set; }
    public Vector3 Velocity {get; set; } = Vector3.zero; 
    public float AngularSpeed_Y {get; set; } = 0f; 
    public Vector3 Linear {get; set; } = Vector3.zero; 
    public float AngularAcceleration_Y {get; set; } = 0f; 
    // public float Rotation_f {get => }
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
        // AgentState = new ArriveState(this);
        AgentState = new AlignState(this, new LookaheadTargetPositionUpdater());

    }

    void FixedUpdate(){
        if(TargetRB != null){
            HandleAgentMovement(Time.fixedDeltaTime);
            // Debug.Log($"Speed: {Velocity.magnitude}");

        }

    }

    void HandleAgentMovement(float time){
        SteeringOutput? steering = AgentState.GetSteering();

        Rb.MovePosition(Rb.position + Velocity * time);
        Rb.MoveRotation(Rb.rotation * Quaternion.AngleAxis(AngularSpeed_Y * time, Vector3.down));
        Velocity = Vector3.ClampMagnitude(Velocity + steering?.linearAcceleration*time ?? Vector3.zero, maxSpeed);
        AngularSpeed_Y = AngularSpeed_Y + steering?.angularAcceleration * time ?? 0f;
        
        //debug
        Linear = steering?.linearAcceleration ?? Vector3.zero;
        AngularAcceleration_Y = steering?.angularAcceleration ?? 0f;
    }

    void AssignTarget(Rigidbody newTarget){
        TargetRB = newTarget;
        Target = Instantiate(targetIndicatorPrefab.transform);
    }
    public void SetState(AgentState state) {
        // if(AgentState != null) yield return StartCoroutine(genericState.OnStateExit());
        AgentState = state;
        // yield return StartCoroutine(genericState.OnStateEnter());
    }
}
