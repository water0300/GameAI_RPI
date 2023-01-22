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
    public PathHandler pathHandler;
    
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

    [Header("Wander Modifiers")]
    public float wanderOffset;
    public float wanderRadius;
    public float wanderRate;

    [Header("Path Following Modifiers")]
    [Range(-3f, 3f)] public float pathOffset = 0.5f;

    public event Action OnStateChange;
    public string statusText {get; set; } =  "Idle";
    public IAgentState ActiveState {get; private set; }
    public List<IAgentState> AgentStateList {get; private set; }
    public Rigidbody TargetRB {get; private set; }
    public Transform Target {get; private set; }
    public Rigidbody Rb {get; private set; }
    public Vector3 Velocity {get; set; } = Vector3.zero; 
    public float AngularSpeed_Y {get; set; } = 0f; 
    public Vector3 Linear {get; set; } = Vector3.zero; 
    public float AngularAcceleration_Y {get; set; } = 0f; 
    public Path Path {get; set; }

    private void OnEnable() {
        // waypointPool.OnWaypointSelect += AssignTarget;
        player.OnTargetActivate += AssignTarget;
        pathHandler.OnPathUpdate += AssignPath;

    }
    private void OnDisable() {
        // waypointPool.OnWaypointSelect -= AssignTarget;
        player.OnTargetActivate -= AssignTarget;
        pathHandler.OnPathUpdate -= AssignPath;
    }
    void Awake(){
        Rb = GetComponent<Rigidbody>();
    }

    void Start(){
        AgentStateList = new List<IAgentState>(){
            new PursueState(),
            new FleeState(),
            new WanderState(),
            new FollowPathState()
        };
        SetState(0); //temp

    }

    void FixedUpdate(){
        if(TargetRB != null){
            HandleAgentMovement(Time.fixedDeltaTime);
        }

    }

    public float currParam = 0f;
    //consider null check
    void HandleAgentMovement(float time){
        SteeringOutput CurrSteeringOutput = SteeringOutputFactory.GetSteering(this, ActiveState);

        Rb.MovePosition(Rb.position + Velocity * time);
        Rb.MoveRotation(Rb.rotation * Quaternion.AngleAxis(AngularSpeed_Y * time, Vector3.down));
        Velocity = Vector3.ClampMagnitude(Velocity + CurrSteeringOutput.linearAcceleration*time ?? Vector3.zero, maxSpeed);
        AngularSpeed_Y += CurrSteeringOutput.angularAcceleration * time ?? 0f;
        
        //debug
        Linear = CurrSteeringOutput.linearAcceleration ?? Vector3.zero;
        AngularAcceleration_Y = CurrSteeringOutput.angularAcceleration ?? 0f;
    }

    void AssignTarget(Rigidbody newTarget){
        TargetRB = newTarget;
        Target = Instantiate(targetIndicatorPrefab.transform);
    }

    void AssignPath(Path path){
        Path = path;
    }

    public void SetState(int index) {
        ActiveState = AgentStateList[index];
        OnStateChange?.Invoke();
        //for now, hard reset
        Velocity = Vector3.zero;
        AngularSpeed_Y = 0f;
        transform.position = Vector3.zero + Vector3.up * 2f;

        //todo instead of hard coding, encode into state
        if(index == 3){
            pathHandler.enabled = true;
        } else {
            pathHandler.enabled = false;
            pathHandler.ClearPath();
        }
    }

}
