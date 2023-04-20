using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Sex { MALE, FEMALE}

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Animal : Resource {
    
    [Header("Movement Attributes")]
    public float maxSpeed;
    [Range(0.01f, 2f)] public float agentArrivalRadius = 1f; 
    [Range(0.01f, 10f)] public float agentWanderSampleRadius = 5f; 
    [Range(0.1f, 1f)] public float agentWanderForwardBias = .5f; 
    [Range(0.1f, 90f)] public float agentWanderForwardAngleRange = 45f; 

    [Header("Health Attributes")]
    public float foodYield = 50;
    public float maxThirst = 100;
    public float maxHunger = 100;
    public float maxMateDesire = 100;

    [field: SerializeField] public Transform Target {get; set; }
    [field: SerializeField] public float CurrentThirst {get; protected set; } 
    [field: SerializeField] public float CurrentHunger {get; protected set; }
    [field: SerializeField] public float CurrentMateDesire {get; protected set; }
    [field: SerializeField] public Sex Sex {get; protected set; }
    [field: SerializeField] public AnimalBehaviorState ActiveState {get; protected set; }

    // [field: SerializeField] public string DebugState {get; set; } = "DEFAULT";
    public NavMeshAgent Agent {get; private set; }
    public Vector3 DebugSetPosition {get; set; }
    public Vector3 DebugTrySetPosition {get; set; }
    
    private void Awake() {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {

        //temp testing
        CurrentThirst -= Time.deltaTime;
        CurrentHunger -= Time.deltaTime;

        if(DecideGoal()){
            Target = null;
        }
        ActiveState.OnUpdate();

    }

    public override float GetConsumed(float amount) {
        //shinei
        
        return foodYield;
    }


    public void Feed(float amount){
        CurrentHunger = Mathf.Min(maxHunger, CurrentHunger + amount);
    }

    public void Drink(float amount){
        CurrentThirst = Mathf.Min(maxThirst, CurrentThirst + amount);
    }

    protected virtual bool DecideGoal(){
        //if hunger and thirst are too low, always prioritize
        float minVal = Mathf.Min(CurrentThirst, CurrentHunger);
        if(CurrentThirst/maxThirst > 0.2f && CurrentHunger/maxHunger > 0.2f){
            minVal = Mathf.Min(minVal, CurrentMateDesire);
        }

        if(Utility.CompareFloats(minVal, CurrentThirst)){
            return SetGoal(new SeekResourceFeedState<WaterResource>(this));
        } else if(Utility.CompareFloats(minVal, CurrentHunger)){
            return SetFoodGoal();
        } else {
            Debug.Log("?????");
            return SetMateGoal();
        }
    }

    protected abstract bool SetFoodGoal();
    protected abstract bool SetMateGoal();


    protected bool SetGoal(AnimalBehaviorState newGoal){
        if (ActiveState != null && ActiveState.GetType() == newGoal.GetType()){
            return false;
        } else {
            ActiveState = newGoal;
            return true;
        }
    }

    private void OnDrawGizmos() {
        Utility.DrawCircle(transform.position, agentArrivalRadius, Color.red);
        Utility.DrawCircle(transform.position, agentWanderSampleRadius, Color.green);
        Utility.DrawWireArc(transform.position, transform.forward, agentWanderForwardAngleRange*2, agentWanderSampleRadius);
        
        if(DebugSetPosition != null)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(DebugSetPosition, 1);

        if(DebugTrySetPosition != null)
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(DebugTrySetPosition, 1.2f);
    }





}


