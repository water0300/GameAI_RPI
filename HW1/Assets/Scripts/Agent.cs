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
        // AgentState = new SeekAndArriveState();
        // AgentState = new FleeState();
        // AgentState = new ArriveState(this);
        // AgentState = new AlignState(this, new FaceTargetRotationUpdater());
        // AgentState = new WanderState(this);
    }

    void FixedUpdate(){
        if(TargetRB != null){
            HandleAgentMovement(Time.fixedDeltaTime);
            // Debug.Log($"Speed: {Velocity.magnitude}");

        }

    }

    void HandleAgentMovement(float time){
        // SteeringOutput steering = AgentStateFactory.GetSteering(this, new ArriveState(new LookaheadTargetPositionUpdater()), new AlignState(new FaceTargetRotationUpdater())); //pursue
        // SteeringOutput steering = AgentStateFactory.GetSteering(this, new FleeSteer(new LookaheadTargetPositionUpdater()), new AlignState(new HideFromTargetRotationUpdater())); //flee
        SteeringOutput steering = AgentStateFactory.GetSteering(this, new WanderSteer());
        // SteeringOutput steering = AgentStateFactory.GetSteering(this, new FollowPathSteer(), new AlignSteer(new FaceTargetRotationUpdater()));
        Rb.MovePosition(Rb.position + Velocity * time);
        Rb.MoveRotation(Rb.rotation * Quaternion.AngleAxis(AngularSpeed_Y * time, Vector3.down));
        Velocity = Vector3.ClampMagnitude(Velocity + steering.linearAcceleration*time ?? Vector3.zero, maxSpeed);
        AngularSpeed_Y += steering.angularAcceleration * time ?? 0f;
        
        //debug
        Linear = steering.linearAcceleration ?? Vector3.zero;
        AngularAcceleration_Y = steering.angularAcceleration ?? 0f;
    }

    void AssignTarget(Rigidbody newTarget){
        TargetRB = newTarget;
        Target = Instantiate(targetIndicatorPrefab.transform);
    }

    void AssignPath(Path path){
        Path = path;
    }
    // public void SetState(AgentState state) {
    //     // if(AgentState != null) yield return StartCoroutine(genericState.OnStateExit());
    //     AgentState = state;
    //     // yield return StartCoroutine(genericState.OnStateEnter());
    // }
}
