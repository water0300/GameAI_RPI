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

    [field: Header("Separation Modifiers")]
    [field: SerializeField] public float Threshold = 6f;
    [field: SerializeField] public float DecayCoefficient = 7f;

    [field: Header("Collision Avoidance Modifiers")]
    [field: SerializeField] [field: Range(-1f, 1f)] public float ConeThreshold = .5f;


    public event Action OnStateChange;
    public string statusText {get; set; } =  "Idle";
    // public ISubState ActiveState {get; private set; }
    public AgentStateComposite ActiveState {get; private set; }
    public List<AgentStateComposite> AgentStateList {get; private set; }
    public Rigidbody TargetRB {get; private set; }
    public Transform Target {get; private set; }
    public Rigidbody Rb {get; private set; }
    public Vector3 Velocity {get; set; } = Vector3.zero; 
    public float AngularSpeed_Y {get; set; } = 0f; 
    public Vector3 Linear {get; set; } = Vector3.zero; 
    public float AngularAcceleration_Y {get; set; } = 0f; 
    public Path Path {get; set; }
    private Vector3 _avgNormal;
    private bool _isColliding = false;

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
        // AgentStateList = new List<ISubState>(){
        //     new PursueSubState(this),
        //     new FleeSubState(this),
        //     new WanderSubState(this),
        //     new FollowPathSubState(this)
        // };
        // AgentStateList = new List<AgentStateComposite>(){
        //     new AgentStateComposite(this, new Dictionary<ISubState, float>(){
        //         {new PursueSubState(this), 1} 
        //     })
        // };
        AgentStateList = new List<AgentStateComposite>(){
            new AgentStateComposite(this, new Dictionary<ISubState, float>(){
                // {new PursueSubState(this), .2f},
                // {new EvadeSubState(this), 1f},

                {new ConeCheckState(this), 1f} 
            })
        };
        SetState(0); //temp

    }

    void FixedUpdate(){
        if(TargetRB != null){
            HandleAgentMovement(Time.fixedDeltaTime);
        }

    }
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.layer == 6){
            return;
        }

        ContactPoint[] contacts = new ContactPoint[other.contactCount];
        _avgNormal = Vector3.zero;
        int contactCount = other.GetContacts(contacts);
        foreach(var c in contacts){
            _avgNormal += c.normal * contactCount;
        }
        _avgNormal.Normalize();
        _isColliding = true;
    } 
    private void OnCollisionExit(Collision other) {
        if(other.gameObject.layer == 6){
            return;
        }
        _isColliding = false;
    }

    void HandleCollision(){
        if(_isColliding){
            // Debug.Log($"normal: {avgNormal} dot: {Vector3.Dot(avgNormal, InputAxis.XZPlane())}");
            float avgDot = Vector3.Dot(_avgNormal, Velocity);
            if(avgDot < 0) { //scale movement based on dot (-1 == no movmeent, -0.01 == some sideways movement)
                Vector3 tangent = Vector3.Cross( _avgNormal, Vector3.up); //respect to y axis
                Velocity = tangent * Vector3.Dot(tangent, Velocity);
            } 
        } 
    }

    void HandleAgentMovement(float time){
        SteeringOutput CurrSteeringOutput = ActiveState.GetSteering();

        HandleCollision();
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
        // transform.position = Vector3.zero + Vector3.up * 2f;
        // player.transform.position = new Vector3(13, 2, 4);//lmao
        pathHandler.ClearPath();

        OnStateChange?.Invoke();
    }

    public void HardReset(){ //todo - put in manager
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDrawGizmos() {
        float deltaTheta = (2f*Mathf.PI) / 40f;
        float theta = 0f;
        Vector3 oldPos = transform.position;
        for(int i = 0; i < 40f; i++){
            Vector3 pos = new Vector3(Threshold * Mathf.Cos(theta), 0f, Threshold * Mathf.Sin(theta));
            Gizmos.DrawLine(oldPos, transform.position + pos);
            oldPos = transform.position + pos;
            theta += deltaTheta;
        }

        ActiveState?.OnDrawGizmo();

    }
    
}
