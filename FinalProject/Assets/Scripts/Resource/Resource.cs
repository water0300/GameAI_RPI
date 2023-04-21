using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Resource : MonoBehaviour {
    public bool IsAlive {get; protected set; } = true;
    public abstract float GetConsumed(float amount);

}
