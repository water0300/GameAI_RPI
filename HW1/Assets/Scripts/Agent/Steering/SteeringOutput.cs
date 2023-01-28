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

    public void ModifyByWeight(float weight){
        linearAcceleration = linearAcceleration * weight ?? Vector3.zero;
        angularAcceleration = angularAcceleration * weight ?? 0f;
    }


}