using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SteeringOutput {
    public Vector2? linearAcceleration;
    public float? angularAcceleration;
    public SteeringOutput(Vector2? linear, float? angular){
        this.linearAcceleration = linear;
        this.angularAcceleration = angular;
    }

    public void ModifyByWeight(float weight){
        linearAcceleration = linearAcceleration * weight ?? Vector2.zero;
        angularAcceleration = angularAcceleration * weight ?? 0f;
    }

}