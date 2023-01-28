using System;
using System.Collections.Generic;
using UnityEngine;

public static class SteeringOutputFactory {
    public static SteeringOutput GetSteering(Agent agent, Dictionary<ISubState, float> behaviors){
        Vector3 r_linear = Vector3.zero;
        float r_angular = 0f;
        foreach(var b in behaviors){
            var steering = b.Key.GetSteering();
            steering.ModifyByWeight(b.Value);
            r_linear += steering.linearAcceleration ?? Vector3.zero;
            r_angular += steering.angularAcceleration ?? 0f;
        }
        r_linear = Vector3.ClampMagnitude(r_linear, agent.MaxAcceleration);
        r_angular = Mathf.Max(r_angular, agent.MaxAngularAcceleration_Y);
        return new SteeringOutput(r_linear, r_angular);
    }

}