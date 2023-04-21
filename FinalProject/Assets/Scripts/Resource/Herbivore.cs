using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//todo herbivore should be abstract
public class Herbivore : Animal
{
    // protected override bool DecideGoal() {
    //     if(Target != null && Target.TryGetComponent<Predator>(out _)){
    //         // return SetGoal(AnimalBehaviorState.FLEE);
    //     } else {
    //         return base.DecideGoal();
    //     }
    // }


    protected override bool SetFoodGoal() => SetGoal(new SeekResourceFeedState<Plant>(this));

    protected override bool SetMateGoal() => SetGoal(new SeekMateState<Herbivore>(this));
}
