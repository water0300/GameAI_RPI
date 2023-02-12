using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Steering {
    SteeringOutput GetSteering(Character agent);
}

public class MatchLeaderSteer : Steering {
    protected Vector2? GetAvoidanceSteering(Character agent){
        RaycastHit2D hit = Physics2D.Raycast(agent.transform.position, agent.transform.up, agent.threshold, ~(1 << 6 | 1 << 3));
        if(hit.collider != null){
            // Debug.Log(hit.collider.bounds.center);
            agent.CollisionIndicatorPoint = hit.transform.position.IgnoreZ();
            Vector2 ahead = agent.transform.position + agent.transform.up * agent.threshold;
            agent.AheadIndicatorPoint = ahead;
            Vector2 avoidanceForce = (ahead - hit.transform.position.IgnoreZ()).normalized * agent.maxAvoidForce;
            agent.AvoidanceForcePoint = avoidanceForce + hit.transform.position.IgnoreZ();
            // agent.
            return avoidanceForce;
        } else {
            agent.CollisionIndicatorPoint = null;
            agent.AheadIndicatorPoint = null;
            // agent.avoi
            return null;
        }
    }
    protected Vector2? GetPositionSteering(Character agent){
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

    protected float? GetRotationSteering(Character agent){
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

        targetAngularSpeed_Y *= -rotation/rotationSize;
        return Mathf.Clamp((targetAngularSpeed_Y - agent.AngularSpeed) / agent.TimeToAlign, -agent.maxAngularAcceleration_Y, agent.maxAngularAcceleration_Y);
    }

    public virtual SteeringOutput GetSteering(Character agent) {
        return new SteeringOutput(GetAvoidanceSteering(agent), GetRotationSteering(agent));
        // return new SteeringOutput(GetPositionSteering(agent) + GetAvoidanceSteering(agent) ?? GetPositionSteering(agent) ?? GetAvoidanceSteering(agent) ?? null, GetRotationSteering(agent));
    }

}


public class LeaderSteer : MatchLeaderSteer {


    public override SteeringOutput GetSteering(Character agent) {
        Vector2 direction =  agent.transform.position.IgnoreZ() - agent.Target.position;
        if(direction.sqrMagnitude != 0){
            agent.Target.orientationDeg = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg; //trial and error
        }
        // return new SteeringOutput(GetPositionSteering(agent), GetRotationSteering(agent));

        return new SteeringOutput(GetPositionSteering(agent) + GetAvoidanceSteering(agent) ?? GetPositionSteering(agent) ?? GetAvoidanceSteering(agent) ?? null, GetRotationSteering(agent));
    }

}