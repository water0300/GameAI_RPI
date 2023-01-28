using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Agent : MonoBehaviour {
    [Header("Prefab References")]
    public GameObject targetIndicatorPrefab;

    [Header("In Scene References")]
    // public WaypointPool waypointPool;
    public Player player;
    public PathHandler pathHandler;

    [field: Header("Position Modifiers")] 
    [field: SerializeField] public float MaxSpeed {get; set; } = 5;
    [field: SerializeField] public float MaxAcceleration {get; set; }= 8f;
    [field: SerializeField] public float TargetRadius {get; set; }= 1.75f;
    [field: SerializeField] public float SlowRadius {get; set; }= 9.4f;
    [field: SerializeField] public float MaxPredictionLookahead {get; set; } = 1.57f;
    [field: SerializeField] [field: Range(0.01f, 0.5f)] public float TimeToTarget {get; set; }= 0.01f;

    [field: Header("Rotation Modifiers")]
    [field: SerializeField] public float MaxAngularSpeed_Y {get; set; } = 72f;
    [field: SerializeField] public float MaxAngularAcceleration_Y {get; set; } = 39f;
    [field: SerializeField] [field: Range(0f, 180f)]public float TargetAlignWindow {get; set; } = 5f;
    [field: SerializeField] [field: Range(0f, 180f)]public float SlowAlignWindow {get; set; } = 50f;
    [field: SerializeField] [field: Range(0.01f, 2f)] public float TimeToAlign {get; set; } = 0.05f;

    [field: Header("Wander Modifiers")]
    [field: SerializeField] public float WanderOffset {get; set; } = 7f;
    [field: SerializeField] public float WanderRadius {get; set; } = 4.8f;
    [field: SerializeField] public float WanderRate {get; set; } = 3.5f;

    [field: Header("Path Following Modifiers")]
    [field: SerializeField] [field: Range(-3f, 3f)] public float PathOffset {get; set; } = 0.2f;

    [field: Header("Collision Avoidance Modifiers")]



    public event Action OnStateChange;
    public string statusText {get; set; } =  "Idle";
    public ISubState ActiveState {get; private set; }
    public List<ISubState> AgentStateList {get; private set; }
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
        AgentStateList = new List<ISubState>(){
            new PursueSubState(this),
            new FleeSubState(this),
            new WanderSubState(this),
            new FollowPathSubState(this)
        };
        SetState(0); //temp

    }

    void FixedUpdate(){
        if(TargetRB != null){
            HandleAgentMovement(Time.fixedDeltaTime);
        }

    }
    // bool AttemptAgentMovement(float time){

    // }

    

    void HandleAgentMovement(float time){
        SteeringOutput CurrSteeringOutput = ActiveState.GetSteering();

        Rb.MovePosition(Rb.position + Velocity * time);
        Rb.MoveRotation(Rb.rotation * Quaternion.AngleAxis(AngularSpeed_Y * time, Vector3.down));
        Velocity = Vector3.ClampMagnitude(Velocity + CurrSteeringOutput.linearAcceleration*time ?? Vector3.zero, MaxSpeed);
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
        if(ActiveState != null){
            ActiveState.OnStateExit();
        }
        ActiveState = AgentStateList[index];
        ActiveState.OnStateEnter();

        //for now, hard reset
        Velocity = Vector3.zero;
        AngularSpeed_Y = 0f;
        transform.position = Vector3.zero + Vector3.up * 2f;
        player.transform.position = new Vector3(13, 2, 4);//lmao
        pathHandler.ClearPath();

        OnStateChange?.Invoke();
    }

    public void HardReset(){ //todo - put in manager
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
