using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// public class FleeState : AnimalBehaviorState
// {
//     public FleeState(Animal animal) : base(animal)
//     {
//     }
//     public override bool CompareGoalToTarget(Collider potentialTarget)
//     {
//         // Debug.Log(potentialTarget.name);
//         return potentialTarget.TryGetComponent<Carnivore>(out _);
       
//     }

//     public override void OnUpdateGoalAcquired()
//     {
//         Animal.DebugState = $"Spotted carnivore, fleeing";
//         Animal.DebugSetPosition = Animal.Target.transform.position;
//         Animal.Agent.SetDestination(Animal.Target.transform.position);
//     }

//     public override string ToString()
//     {
//         return $"Seeking Resource {typeof(TResource)}";
//     }

// }