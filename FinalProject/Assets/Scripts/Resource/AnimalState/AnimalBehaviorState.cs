using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AnimalBehaviorState {
    public Animal Animal {get; set; }
    public abstract bool CompareGoalToTarget(Collider potentialTarget);
    public abstract void OnUpdateGoalAcquired();
    public bool needToFlee = false;
    public AnimalBehaviorState(Animal animal){
        Animal = animal;
    }


    public void OnUpdate(){
        if(!Animal.IsAlive){
            return;
        }

        if(Animal.Target == null){
            Animal.DebugState = $"Wandering, {ToString()}";
            Wander();
        } else {
            if(needToFlee){
                Animal.DebugState = $"Fleeing from predator";
                OnFleeUpdate();
            } else {
                OnUpdateGoalAcquired();
            }
        }

    }

    bool cycleSampledPosition = true; 
    private void Wander(){
        // Debug.Log($"cycle: {cycleSampledPosition}");
        if(cycleSampledPosition){
            Animal.DebugSetPosition = SampleProximatePosition();
            Animal.Agent.SetDestination(Animal.DebugSetPosition);
            cycleSampledPosition = false;
        }

        if (!Animal.Agent.pathPending && Animal.Agent.remainingDistance < Animal.agentArrivalRadius) {
            // The agent has reached its destination, EVEN IF THEY"RE NOT WANDERING (side effect)
            // Debug.Log($"reach dest???");

            cycleSampledPosition = true;
        }
    }

    public bool CheckForPredator(Collider potentialTarget){
        return Animal is Herbivore && potentialTarget.TryGetComponent<Carnivore>(out _);
    }

    private void OnFleeUpdate(){
        // Debug.Log("FLEEING");
        //randomly pick a position away from the target

        Vector3 fleeDirection = (Animal.transform.position - Animal.Target.position).normalized;
        // fleeDirection *= -1;
        Animal.DebugSetPosition = Animal.transform.position + fleeDirection;

        //  = newDestination;

        NavMesh.SamplePosition(Animal.DebugSetPosition, out NavMeshHit hit, Animal.agentWanderSampleRadius, NavMesh.AllAreas);
        // NavMesh.SamplePosition(transform.position, out NavMeshHit hit, agentWanderSampleRadius, NavMesh.AllAreas);
        if(hit.hit == false){
            Debug.LogWarning("No flee location found, give up");
        } else {
            Animal.Agent.SetDestination(Animal.DebugSetPosition);
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
            Debug.LogWarning("No valid sample found, handle recursively...");
            return SampleProximatePosition();
        }

        return hit.position;
    }


}

