using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour {
    public Transform tempTarget;
    public bool manual = false;
    public bool leader = false;
    [Header("Rotational")]
    public float TimeToAlign = .5f;
    [Header("Obstacal")]
    public float threshold = 1f;
    public float maxAvoidForce = 5f;

    [field: SerializeField] public Path Path {get; set; }
    public PositionOrientation Target {get; private set; }
    public Rigidbody2D Rb {get; private set; }
    public NavMeshAgent Agent {get; private set; }
    public Collider2D Col {get; private set; }
    public FormationManager Fm {get; private set; }
    public Vector2 Velocity {get; set; }
    public float AngularSpeed {get; set; }

    //debug
    public Vector2 Linear {get; private set; }
    public float AngularAcceleration {get; private set; }
    public Vector2? CollisionIndicatorPoint {get; set; }
    public Vector2? CollisionAheadPoint {get; set; }
    public Vector2? AvoidanceForcePoint {get; set; }
    private float initSpreadAngle;
    void Awake(){
        Col = GetComponent<Collider2D>();
        Agent = GetComponent<NavMeshAgent>();
        Fm = FindObjectOfType<FormationManager>();
    }
    private void Start() {
        // Steer = new MatchLeaderSteer();
        // Rb = GetComponent<Rigidbody2D>();
        Agent.updateRotation = false; //todo check
        Agent.updateUpAxis = false;
        if(manual)
            SetTarget(new PositionOrientation(tempTarget.position, tempTarget.rotation.eulerAngles.z));

        if(leader){
            Path = FindObjectOfType<Path>();
            // Steer = new LeaderSteer();
        } else {
            // Steer = new MatchLeaderSteer();
        }

        initSpreadAngle = Fm.spreadAngle;
    }

    public void SetTarget(PositionOrientation target){
        Target = target;
        Agent.SetDestination(target.position);
    }

    Vector3 _vPos;
    float oldAng;
    float _m;
    int currNode = 0;
    private void Update() {
        if(manual) {
            SetTarget(new PositionOrientation(tempTarget.position, tempTarget.rotation.eulerAngles.z));
            Agent.updatePosition = false;
        }
        if(leader){
            PositionOrientation target = new PositionOrientation();

            if(Path != null){
                target.position = Path.nodes[currNode].transform.position;
                if(Vector2.Distance(transform.position, target.position) <= 2f){
                    currNode++;
                    if(currNode >= Path.nodes.Count) {
                        currNode = Path.nodes.Count - 1;
                    }
                }
            }
            SetTarget(target);

            Vector2 direction = (Target.position - transform.position.IgnoreZ()).normalized;
            float angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
            float newAng = Mathf.SmoothDampAngle(oldAng, angle, ref _m, TimeToAlign);

            transform.rotation = Quaternion.Euler(0f, 0f, newAng);
            oldAng = newAng;

            //manual for now
            if(currNode == 5){
                Fm.spreadAngle = 90;
            }

            if(currNode == 10){
                Fm.spreadAngle = initSpreadAngle;
            }

        } else {
            // if(Fm.leader == null){ //fm needs to be injected?
            //     Debug.Log("???");
            //     return;
            // }
            float newAng = Mathf.SmoothDampAngle(oldAng, Fm.leader.transform.eulerAngles.z, ref _m, TimeToAlign);
            transform.rotation = Quaternion.Euler(0f, 0f, newAng);
            oldAng = newAng;

        }
        transform.rotation = Quaternion.Euler(0f, 0f, transform.eulerAngles.z);
    }



    void HandleAgentMovement(float time){
        
        // SteeringOutput CurrSteeringOutput = Steer.GetSteering(this);

        // Rb.MovePosition(Rb.position + Velocity * time);
        // if(CurrSteeringOutput.linearAcceleration != null){
        //     Rb.AddForce(CurrSteeringOutput.linearAcceleration.Value);
        // } else {
        //     Rb.velocity = Vector2.zero;
        // }

        // if(CurrSteeringOutput.angularAcceleration != null){
        //     Rb.AddTorque(CurrSteeringOutput.angularAcceleration.Value);
        // } else {
        //     Rb.angularVelocity = 0f;
        // }


        // Rb.MoveRotation(Rb.rotation * CurrSteeringOutput.angularAcceleration.Value * time);

        // Velocity = Vector2.ClampMagnitude(Velocity + CurrSteeringOutput.linearAcceleration*time ?? Vector3.zero, maxSpeed);
        // AngularSpeed = AngularSpeed + CurrSteeringOutput.angularAcceleration * time ?? 0f;
        
        //debug
        // Linear = CurrSteeringOutput.linearAcceleration ?? Vector3.zero;
        // AngularAcceleration = CurrSteeringOutput.angularAcceleration ?? 0f;
    }

    private Vector3 _lastVector = Vector3.zero;
    private Vector3 _v;
    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        if(Target != null)
            Gizmos.DrawWireSphere(Target.position, Col.bounds.size.x);

        // Gizmos.color = Color.green;
        // Gizmos.DrawWireSphere(transform.position, targetRadius);
        // Gizmos.DrawWireSphere(transform.position, slowRadius);

        // Gizmos.color = Color.white;
        // Vector2 newVec = Vector3.SmoothDamp(_lastVector, Linear, ref _v, 0.5f);
        // Gizmos.DrawLine(transform.position, transform.position.IgnoreZ() + newVec);
        // _lastVector = newVec;

        // if(CollisionIndicatorPoint != null){
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawLine(transform.position, CollisionIndicatorPoint.Value);
        //     if(AvoidanceForcePoint != null){
        //         Gizmos.color = Color.blue;
        //         Gizmos.DrawLine(CollisionIndicatorPoint.Value, AvoidanceForcePoint.Value);
        //     }
        // }

        // if(CollisionAheadPoint != null){
        //     Gizmos.color = Color.cyan;
        //     Gizmos.DrawLine(transform.position, CollisionAheadPoint.Value);

        // }
        // if(Col != null){
        //     Vector2 b1 = Col.bounds.center + transform.right * (Col.bounds.size.x/2);
        //     Vector2 b2 = Col.bounds.center - transform.right * (Col.bounds.size.x/2);
        //     Gizmos.color = Color.magenta;
        //     Gizmos.DrawLine(b1, b1 + threshold * transform.up.IgnoreZ());
        //     Gizmos.DrawLine(b2, b2 + threshold * transform.up.IgnoreZ());
        // }

            // Debug.Log(Col.bounds.min);
            
        

    }
}
