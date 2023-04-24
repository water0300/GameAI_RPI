using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carnivore : Animal
{

    protected override bool SetFoodGoal()
    {
        return SetGoal(new SeekResourceFeedState<Herbivore>(this));
    }

    protected override bool SetMateGoal()
    {
        return SetGoal(new SeekMateState<Carnivore>(this));
    }
}
