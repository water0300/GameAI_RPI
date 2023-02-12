using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour {
    public Transform tempTarget;
    public bool manual = false;
    public bool leader = false;
    [Header("Positional")]
    public float maxSpeed = 5f;
    public float maxAcceleration = 3f;
    public float targetRadius = 3f;
    public float slowRadius = 10f;
    public float timeToTarget = 0.01f;
    [Header("Rotational")]
    public float maxAngularSpeed_Y = 90f;
    public float maxAngularAcceleration_Y = 70f;
    public float targetAlignWindow = 30f;
    public float slowAlignWindow = 100f;
    public float TimeToAlign = 0.01f;
    [Header("Obstacal")]
    public float threshold = 1f;
    public float maxAvoidForce = 5f;

    [field: SerializeField] public Path Path {get; set; }
    public PositionOrientation Target {get; set; }
    public Rigidbody2D Rb {get; private set; }
    public Collider2D Col {get; private set; }
    public Steering Steer {get; private set; }
    public Vector2 Velocity {get; set; }
    public float AngularSpeed {get; set; }

    //debug
    public Vector2 Linear {get; private set; }
    public float AngularAcceleration {get; private set; }
    public Vector2? CollisionIndicatorPoint {get; set; }
    public Vector2? CollisionAheadPoint {get; set; }
    public Vector2? AvoidanceForcePoint {get; set; }
    private void Start() {
        // Steer = new MatchLeaderSteer();
        Steer = new LeaderSteer();
        Rb = GetComponent<Rigidbody2D>();
        Col = GetComponent<Collider2D>();
        if(manual)
            Target = new PositionOrientation(tempTarget.position, tempTarget.rotation.eulerAngles.z);

        if(leader){
            Path = FindObjectOfType<Path>();
        }
    }

    private void Update() {
        if(manual)
            Target = new PositionOrientation(tempTarget.position, tempTarget.rotation.eulerAngles.z);
    }

    private void FixedUpdate() {
        HandleAgentMovement(Time.fixedDeltaTime);
    }

    void HandleCollision(){

    }

    void HandleAgentMovement(float time){
        SteeringOutput CurrSteeringOutput = Steer.GetSteering(this);

        HandleCollision();
        // Rb.MovePosition(Rb.position + Velocity * time);
        if(CurrSteeringOutput.linearAcceleration != null){
            Rb.AddForce(CurrSteeringOutput.linearAcceleration.Value);
        } else {
            Rb.velocity = Vector2.zero;
        }

        if(CurrSteeringOutput.angularAcceleration != null){
            Rb.AddTorque(CurrSteeringOutput.angularAcceleration.Value);
        } else {
            Rb.angularVelocity = 0f;
        }


        // Rb.MoveRotation(Rb.rotation * CurrSteeringOutput.angularAcceleration.Value * time);

        // Velocity = Vector2.ClampMagnitude(Velocity + CurrSteeringOutput.linearAcceleration*time ?? Vector3.zero, maxSpeed);
        // AngularSpeed = AngularSpeed + CurrSteeringOutput.angularAcceleration * time ?? 0f;
        
        //debug
        Linear = CurrSteeringOutput.linearAcceleration ?? Vector3.zero;
        AngularAcceleration = CurrSteeringOutput.angularAcceleration ?? 0f;
    }

    private Vector3 _lastVector = Vector3.zero;
    private Vector3 _v;
    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        if(Target != null)
            Gizmos.DrawWireSphere(Target.position, targetRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, targetRadius);
        Gizmos.DrawWireSphere(transform.position, slowRadius);

        Gizmos.color = Color.white;
        Vector2 newVec = Vector3.SmoothDamp(_lastVector, Linear, ref _v, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position.IgnoreZ() + newVec);
        _lastVector = newVec;

        if(CollisionIndicatorPoint != null){
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, CollisionIndicatorPoint.Value);
            if(AvoidanceForcePoint != null){
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(CollisionIndicatorPoint.Value, AvoidanceForcePoint.Value);
            }
        }

        if(CollisionAheadPoint != null){
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, CollisionAheadPoint.Value);

        }
        if(Col != null){
            Vector2 b1 = Col.bounds.center + transform.right * (Col.bounds.size.x/2);
            Vector2 b2 = Col.bounds.center - transform.right * (Col.bounds.size.x/2);
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(b1, b1 + threshold * transform.up.IgnoreZ());
            Gizmos.DrawLine(b2, b2 + threshold * transform.up.IgnoreZ());
        }

            // Debug.Log(Col.bounds.min);
            
        

    }
}
