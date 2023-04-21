using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//STATIC = LIFESPAN SHOULD BE ONE MINUTE?

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Animal : Resource {
    
    [Header("Generic Genes")]
    public float maxSpeed; 
    [Range(0.1f, 1f)] public float agentWanderForwardBias = .8f; 
    public float metabolism; //todo: how quicky you consume this stuff
    public float foodYield = 50;
    public float detectionRadius = 12;

    [Header("Movement Attributes")]
    [Range(0.01f, 2f)] public float agentArrivalRadius = 1f; 
    [Range(0.01f, 10f)] public float agentWanderSampleRadius = 5f; 
    [Range(0.1f, 90f)] public float agentWanderForwardAngleRange = 45f; 

    [Header("Health Attributes")]
    public float maxThirst = 100;
    public float maxHunger = 100;
    public float maxMateDesire = 100;

    [field: Header("Info")]
    [field: SerializeField] public float CurrentThirst {get; protected set; } 
    [field: SerializeField] public float CurrentHunger {get; protected set; }
    [field: SerializeField] public float CurrentMateDesire {get; protected set; }
    [field: SerializeField] public AnimalBehaviorState ActiveState {get; protected set; }

    public Sex Sex {get; private set; }
    public NavMeshAgent Agent {get; private set; }

    [field: SerializeField] public Transform Target {get; set; }
    public Vector3 DebugSetPosition {get; set; }
    public Vector3 DebugTrySetPosition {get; set; }
    [field: SerializeField] public string DebugState {get; set; } = "DEFAULT";
    
    private void Awake() {
        Agent = GetComponent<NavMeshAgent>();

        bool isM = TryGetComponent<Male>(out Male m);
        bool isFm = TryGetComponent<Female>(out Female fm);
        
        if(isM ^ isFm){
            Sex = isM ? m : fm;
        } else {
            Debug.LogError("Check sex assignment?");
        }

        
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
            // Debug.Log("?????");
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

    public void Mate(Animal partner){
        if(Sex is Female){
            //handle pregnancy here
            Debug.Log("PREGNANT NOW");
            (Sex as Female).GeneAbstraction = new GeneAbstraction(partner, this);
            (Sex as Female).InitPregnancyHandler();
        }

        CurrentMateDesire = maxMateDesire;
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


