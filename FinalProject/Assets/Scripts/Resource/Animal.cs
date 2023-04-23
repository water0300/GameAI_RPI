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
    public float metabolism = .5f; //higher = need more food
    public float foodYield = 50;
    public float detectionRadius = 12;

    [Header("Movement Attributes")]
    [Range(0.01f, 2f)] public float agentArrivalRadius = 1f; 
    [Range(0.01f, 10f)] public float agentWanderSampleRadius = 9f; 
    [Range(0.1f, 90f)] public float agentWanderForwardAngleRange = 45f; 

    [Header("Health Attributes")]
    public float maxThirst = 20;
    public float maxHunger = 20;
    public float maxMateDesire = 20;

    [field: Header("Info")]
    [field: SerializeField] public float CurrentThirst {get; protected set; } 
    [field: SerializeField] public float CurrentHunger {get; protected set; }
    [field: SerializeField] public float CurrentMateDesire {get; protected set; }
    [field: SerializeField] public AnimalBehaviorState ActiveState {get; protected set; }
    [field: SerializeField] public float Age {get; private set; } = 0;

    [field: SerializeField] public float InfancyDuration {get; set; }= 10;
    [SerializeField] private bool _isInfant => Age <= InfancyDuration;

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

    private bool _gestationSurvivalCheck = true;

    private void Update() {

        //death checks

        Age += Time.deltaTime;

        GestationSurvivalCheck();

        DeathCheck();

        AdjustHealthStats();

        if(DecideGoal()){
            Target = null;
        }
        ActiveState.OnUpdate();

    }

    void GestationSurvivalCheck(){
        if(_gestationSurvivalCheck && !_isInfant){
            float c = InfancyDuration / GameManager.Instance.lifetimeBoundSecs; //smaller = less likely to succeed
            if(Random.value < Mathf.Clamp01(5.5f*c*c+c+.2f)){ //arbitrary function of choice
                IsAlive = false;
                GameManager.Instance.HandleDeath(this);
            }

            _gestationSurvivalCheck = false;
        }
    }

    void DeathCheck(){
        if(Age >= GameManager.Instance.lifetimeBoundSecs || CurrentHunger <= 0f || CurrentThirst <= 0f){
            IsAlive = false;
            GameManager.Instance.HandleDeath(this);
        }
    }

    void AdjustHealthStats(){
        //as a function of speed
        float speed = Agent.velocity.magnitude;

        Debug.Log(Time.deltaTime * speed * metabolism);

        CurrentThirst -= Time.deltaTime * speed * metabolism;
        CurrentHunger -= Time.deltaTime * speed * metabolism;
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
        if(!_isInfant && CurrentThirst/maxThirst > 0.36f && CurrentHunger/maxHunger > 0.36f){
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


