using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SeekMateState<TAnimal> : AnimalBehaviorState where TAnimal : Animal
{

    // public TAnimal mate;
    // public bool mateFound => mate != null;
    public SeekMateState(Animal animal) : base(animal)
    {
    }



    public override bool CompareGoalToTarget(Collider potentialTarget)
    {
        //only initiate mating with those who also seek to mate (the same species)
        if(potentialTarget.TryGetComponent<TAnimal>(out TAnimal potentialMate) && potentialMate.ActiveState is SeekMateState<TAnimal>){
            
            // SeekMateState<TAnimal> potentialMateState = potentialMate.ActiveState as SeekMateState<TAnimal>;

            //if they've already found a mate, check if its you
            if(potentialMate.Target != null){
                return Animal.transform == potentialMate.Target;
            } 

            //check gender
            if(Animal.Sex.GetType() == potentialMate.Sex.GetType()){
                return false;
            } 

            //otherwise, decide to mate or not
            if(
                Animal.Sex is Male && (potentialMate.Sex as Female).RequestMate(Animal.Sex as Male)
                || Animal.Sex is Female && (Animal.Sex as Female).RequestMate(potentialMate.Sex as Male)
            ){
                potentialMate.Target = Animal.transform;
                return true;
            } else {
                return false;
            }

        } else {
            return false;
        }

    }

    public override void OnUpdateGoalAcquired()
    {
        Animal.DebugState = $"Found Mate {typeof(TAnimal)}, approaching";

        Animal.DebugSetPosition = Animal.Target.transform.position;
        Animal.Agent.SetDestination(Animal.Target.transform.position);

        if (!Animal.Agent.pathPending && Animal.Agent.remainingDistance < Animal.agentArrivalRadius) {
            // The agent has reached its mate
            TAnimal mate = Animal.Target.GetComponent<TAnimal>();
            Animal.Mate();
            mate.Mate();
            //todo: as of this point, goal should be changed, but consider forcing a rethink?
        }

    }

    public override string ToString()
    {
        return $"Seeking Mate {typeof(TAnimal)}";
    }
}