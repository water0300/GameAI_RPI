using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterResource : Resource
{
    public override float GetConsumed(float amount)
    {
        return amount;
    }
}
