using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : AnimalBehaviorState
{

    public WanderState(Animal animal) : base(animal)
    {
    }
    public override bool CompareGoalToTarget(Collider potentialTarget) => false;

    public override void OnUpdateGoalAcquired()
    {
        throw new System.NotImplementedException();
    }

    public override string ToString()
    {
        return "Idle Wandering";
    }
}
