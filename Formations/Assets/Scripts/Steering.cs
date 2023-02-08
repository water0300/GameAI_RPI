using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Steering {
    SteeringOutput GetSteering(Character agent);
}

public class MatchLeaderSteer : Steering {
    public Vector2? GetPositionSteering(Character agent){
        Vector2 direction = agent.Target.position - agent.transform.position.IgnoreZ();
        float distSqrMagnitude = direction.sqrMagnitude;
        if (distSqrMagnitude < agent.targetRadius * agent.targetRadius){
            Debug.Log("wot");
            return null;
        }

        float targetSpeed;
        if(distSqrMagnitude > agent.slowRadius * agent.slowRadius ){
            targetSpeed = agent.maxSpeed;


        } else {
            Debug.Log("Slowing");
            targetSpeed = agent.maxSpeed * Mathf.Sqrt(distSqrMagnitude) / agent.slowRadius;
        }
        Vector2 targetVelocity = direction.normalized * targetSpeed;

        return Vector2.ClampMagnitude((targetVelocity - agent.Rb.velocity) / agent.timeToTarget, agent.maxAcceleration);

    }

    public float? GetRotationSteering(Character agent){
        float rotation = agent.transform.rotation.eulerAngles.z - agent.Target.orientationDeg;
        rotation %= 360;
        rotation = rotation > 180 ? rotation - 360 : (rotation < -180 ? rotation + 360 : rotation);
        float rotationSize = Mathf.Abs(rotation);
        if(rotationSize < agent.targetAlignWindow){
            return null;
        }
        // Debug.Log($"Rotation: {rotation}");

        float targetAngularSpeed_Y;
        if(rotationSize > agent.slowAlignWindow){
            targetAngularSpeed_Y = agent.maxAngularSpeed_Y; 
        } else {
            targetAngularSpeed_Y = agent.maxAngularSpeed_Y * rotationSize / agent.slowAlignWindow;
        }
        // Debug.Log($"TargetRotation: {targetRotation}");

        targetAngularSpeed_Y *= rotation/rotationSize;
        return Mathf.Clamp((targetAngularSpeed_Y - agent.AngularSpeed) / agent.TimeToAlign, -agent.maxAngularAcceleration_Y, agent.maxAngularAcceleration_Y);
    }

    public SteeringOutput GetSteering(Character agent) {
        return new SteeringOutput(GetPositionSteering(agent), GetRotationSteering(agent));
    }

}