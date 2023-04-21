using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SeekResourceFeedState<TResource> : AnimalBehaviorState where TResource : Resource
{
    public SeekResourceFeedState(Animal animal) : base(animal)
    {
    }
    public override bool CompareGoalToTarget(Collider potentialTarget)
    {
        // Debug.Log(potentialTarget.name);
        return potentialTarget.TryGetComponent<TResource>(out _);
       
    }

    public override void OnUpdateGoalAcquired()
    {
        Animal.DebugState = $"Found resource {typeof(TResource)}, approaching";
        Animal.DebugSetPosition = Animal.Target.transform.position;
        Animal.Agent.SetDestination(Animal.Target.transform.position);

        if (!Animal.Agent.pathPending && Animal.Agent.remainingDistance < Animal.agentArrivalRadius) {
            // The agent has reached its destination
            var resource = Animal.Target.GetComponent<TResource>();
            float yield = resource.GetConsumed(Animal.maxHunger - Animal.CurrentHunger);
            Animal.Feed(yield);
            //todo: as of this point, goal should be changed, but consider forcing a rethink?
        }
    }

    public override string ToString()
    {
        return $"Seeking Resource {typeof(TResource)}";
    }

}