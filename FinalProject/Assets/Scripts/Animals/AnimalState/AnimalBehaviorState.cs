using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AnimalBehaviorState {
    public Animal Animal {get; set; }
    public abstract bool CompareGoalToTarget(Collider potentialTarget);
    public abstract void OnUpdateGoalAcquired();

    public AnimalBehaviorState(Animal animal){
        Animal = animal;
    }


    public void OnUpdate(){
        if(Animal.Target == null){
            Wander();
        } else {
            OnUpdateGoalAcquired();
        }

    }

    bool cycleSampledPosition = true; 
    private void Wander(){
        if(cycleSampledPosition){
            Animal.DebugSetPosition = SampleProximatePosition();
            Animal.Agent.SetDestination(Animal.DebugSetPosition);
            cycleSampledPosition = false;
        }

        if (!Animal.Agent.pathPending && Animal.Agent.remainingDistance < Animal.agentArrivalRadius) {
            // The agent has reached its destination
            cycleSampledPosition = true;
        }
    }


    private Vector3 SampleProximatePosition(){

        //randomly pick a forward position
        float targetAngle = Random.value > Animal.agentWanderForwardBias ? Mathf.Lerp(-180, 180, Random.value) :  Mathf.Lerp(-Animal.agentWanderForwardAngleRange, Animal.agentWanderForwardAngleRange, Random.value);
        Vector3 rotatedVector = Quaternion.AngleAxis(targetAngle, Vector3.up) * Animal.transform.forward;

        Animal.DebugTrySetPosition = Animal.transform.position + rotatedVector * Animal.agentWanderSampleRadius;

        NavMesh.SamplePosition(Animal.transform.position + rotatedVector * Animal.agentWanderSampleRadius, out NavMeshHit hit, Animal.agentWanderSampleRadius, NavMesh.AllAreas);
        // NavMesh.SamplePosition(transform.position, out NavMeshHit hit, agentWanderSampleRadius, NavMesh.AllAreas);
        if(hit.hit == false){
            Debug.LogError("No valid sample found, handle eventually...");
        }

        return hit.position;
    }
}


public class SeekWaterState : AnimalBehaviorState
{

    public SeekWaterState(Animal animal) : base(animal)
    {
    }
    public override bool CompareGoalToTarget(Collider potentialTarget)
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdateGoalAcquired()
    {
        throw new System.NotImplementedException();
    }
}

public class SeekFoodState : AnimalBehaviorState
{
    public SeekFoodState(Animal animal) : base(animal)
    {
    }

    public override bool CompareGoalToTarget(Collider potentialTarget)
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdateGoalAcquired()
    {
        throw new System.NotImplementedException();
    }
}



public class SeekMateState : AnimalBehaviorState
{
    public SeekMateState(Animal animal) : base(animal)
    {
    }

    public override bool CompareGoalToTarget(Collider potentialTarget)
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdateGoalAcquired()
    {
        throw new System.NotImplementedException();
    }
}

// public class FleeState : AnimalBehaviorState {
    
// }