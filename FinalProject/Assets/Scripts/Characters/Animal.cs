using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Goal { FOOD, WATER, MATE, FLEE}
public enum Sex { MALE, FEMALE}

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Animal : MonoBehaviour {
    
    [Header("Movement Attributes")]
    public float maxSpeed;
    [Range(0.01f, 2f)] public float agentArrivalRadius = 1f; 
    [Range(0.01f, 10f)] public float agentWanderSampleRadius = 5f; 
    [Range(0.01f, 5f)] public float agentWanderForwardBias = 1f; 

    [Header("Health Attributes")]
    public float maxThirst = 100;
    public float maxHunger = 100;
    public float maxMateDesire = 100;

    [field: SerializeField] public GameObject Target {get; protected set; }
    [field: SerializeField] public float CurrentThirst {get; protected set; } 
    [field: SerializeField] public float CurrentHunger {get; protected set; }
    [field: SerializeField] public float CurrentMateDesire {get; protected set; }
    [field: SerializeField] public Sex Sex {get; protected set; }
    [field: SerializeField] public Goal ActiveGoal {get; protected set; }

    private NavMeshAgent _agent;
    private Vector3 _debugSetPosition;
    
    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if(DecideGoal()){
            Act();
        }        
    }

    protected virtual bool DecideGoal(){
        //if hunger and thirst are too low, always prioritize
        float minVal = Mathf.Min(CurrentThirst, CurrentHunger);
        if(CurrentThirst/maxThirst > 0.2f && CurrentHunger/maxHunger > 0.2f){
            minVal = Mathf.Min(minVal, CurrentMateDesire);
        }

        if(Utility.CompareFloats(minVal, CurrentThirst)){
            return SetGoal(Goal.WATER);
        }else if(Utility.CompareFloats(minVal, CurrentHunger)){
            return SetGoal(Goal.FOOD);
        }else {
            return SetGoal(Goal.MATE);
        }
    }

    protected bool SetGoal(Goal newGoal){
        if (ActiveGoal == newGoal){
            return false;
        } else {
            ActiveGoal = newGoal;
            return true;
        }
    }

    IEnumerator actRoutine;

    //triggers ONCE upon goal switch
    protected void Act(){
        if(actRoutine != null){
            StopCoroutine(actRoutine);
        }

        switch(ActiveGoal){
            case Goal.FOOD:
                actRoutine = FoodRoutine();
                break;
            case Goal.WATER:
                actRoutine = WaterRoutine();
                break;
            case Goal.MATE:
                actRoutine = MateRoutine();
                break;
            case Goal.FLEE:
                actRoutine = FleeRoutine();
                break;
        }

        StartCoroutine(actRoutine);
    }

    protected IEnumerator WaterRoutine(){
        bool cycleSampledPosition = true; 
        while(Target == null){
            if(cycleSampledPosition){
                _debugSetPosition = SampleProximatePosition();
                _agent.SetDestination(_debugSetPosition);
                cycleSampledPosition = false;
            }

            if (!_agent.pathPending && _agent.remainingDistance < agentArrivalRadius) {
                // The agent has reached its destination
                cycleSampledPosition = true;
            }

            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    protected abstract IEnumerator MateRoutine();
    protected abstract IEnumerator FoodRoutine();
    protected virtual IEnumerator FleeRoutine(){ yield return null; }


    protected bool Wander(){
        return false;
    }

    private Vector3 SampleProximatePosition(){
        NavMesh.SamplePosition(transform.position + transform.forward * agentWanderForwardBias, out NavMeshHit hit, agentWanderSampleRadius, NavMesh.AllAreas);
        // NavMesh.SamplePosition(transform.position, out NavMeshHit hit, agentWanderSampleRadius, NavMesh.AllAreas);

        if(hit.hit == false){
            Debug.Log("No valid sample found, recalculating...");
            NavMesh.SamplePosition(transform.position + transform.forward * agentWanderForwardBias, out hit, agentWanderSampleRadius, NavMesh.AllAreas);
        }

        return hit.position;
    }

    private void OnDrawGizmos() {
        DrawCircle(transform.position, agentArrivalRadius, Color.red);
        DrawCircle(transform.position + transform.forward * agentWanderForwardBias, agentWanderSampleRadius, Color.green);
        
        if(_debugSetPosition != null)
            Gizmos.DrawWireSphere(_debugSetPosition, 1);
    }

    private void DrawCircle(Vector3 center, float radius, Color color, float resolution = 40f){
        Gizmos.color = color;
        float deltaTheta = (2f*Mathf.PI) / resolution;
        float theta = 0f;
        Vector3 oldPos = center;
        for(int i = 0; i < resolution; i++){
            Vector3 pos = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
            Gizmos.DrawLine(oldPos, center + pos);
            oldPos = center + pos;
            theta += deltaTheta;
        }
    }

}


