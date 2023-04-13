using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : Animal
{
    protected override bool DecideGoal() {
        if(Target != null && Target.TryGetComponent<Predator>(out _)){
            return SetGoal(AnimalBehaviorState.FLEE);
        } else {
            return base.DecideGoal();
        }
    }

    protected override IEnumerator FoodRoutine()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator MateRoutine()
    {
        throw new System.NotImplementedException();
    }
}
