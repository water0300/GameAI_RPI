using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SteeringOutput {
    public Vector3? linearAcceleration;
    public float? angularAcceleration;
    public SteeringOutput(Vector3? linear, float? angular){
        this.linearAcceleration = linear;
        this.angularAcceleration = angular;
    }

}