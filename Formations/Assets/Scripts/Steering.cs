using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Steering {
    SteeringOutput GetSteering(Character agent);
}

public class MatchLeaderSteer : Steering {
    protected Vector2? GetAvoidanceSteering(Character agent){
        Vector2 p1 = agent.Col.bounds.center + agent.transform.right * (agent.Col.bounds.size.x/2);
        Vector2 p2 = agent.Col.bounds.center - agent.transform.right * (agent.Col.bounds.size.x/2);
        // RaycastHit2D hit0 = Physics2D.Raycast(agent.transform.position, agent.transform.up, agent.threshold, ~(1 << 6 | 1 << 3));
        // RaycastHit2D hit1 = Physics2D.Raycast(p1, agent.transform.up, agent.threshold, ~(1 << 6 | 1 << 3));
        // RaycastHit2D hit2 = Physics2D.Raycast(p2, agent.transform.up, agent.threshold, ~(1 << 6 | 1 << 3));
        // Collider2D colHit = hit0.collider ?? hit1.collider ?? hit2.collider ?? null;
        Collider2D colHit = 
            Physics2D.Raycast(agent.transform.position, agent.transform.up, agent.threshold, ~(1 << 6 | 1 << 3)).collider ??
            Physics2D.Raycast(p1, agent.transform.up, agent.threshold, ~(1 << 6 | 1 << 3)).collider ??
            Physics2D.Raycast(p2, agent.transform.up, agent.threshold, ~(1 << 6 | 1 << 3)).collider ??
            null;


        if(colHit != null){
            // Debug.Log(colHit.gameObject.name);
            agent.CollisionIndicatorPoint = colHit.transform.position.IgnoreZ();
            // agent.CollisionAheadPoint = agent.transform.position + agent.transform.up * agent.threshold;
            agent.CollisionAheadPoint = agent.Target.position;

            Vector2 v1 = (agent.transform.position.IgnoreZ() - agent.CollisionIndicatorPoint.Value).normalized;
            Vector2 v2 = (agent.transform.position.IgnoreZ() - agent.CollisionAheadPoint.Value).normalized;
            // var dot = Vector2.Dot(new Vector2(v1.x, -v1.y), new Vector2(-v2.y, v2.x));
            var cross = Vector3.Cross(v1, v2);
            // Debug.Log(cross);
            // Debug.Log(colHit.bounds.ClosestPoint(agent.transform.position));
            float lOrRMod = 0;
            if(cross.z > 0){ //closest point is right of center
                lOrRMod = 1;
                // Debug.Log("left of center");
            } else {
                lOrRMod = -1;
                // Debug.Log("right of center");

            }
            Vector2 direction = (agent.transform.position - colHit.transform.position);
            Vector2 tangent = Vector3.Cross(direction.normalized, Vector3.forward); //respect to y axis
            Vector2 avoidanceForce = agent.maxAvoidForce * tangent * lOrRMod;
            // Debug.Log(avoidanceForce);
            agent.AvoidanceForcePoint = avoidanceForce + colHit.transform.position.IgnoreZ();

            return avoidanceForce;


            // agent.
            // return avoidanceForce;
        } else {
            agent.CollisionIndicatorPoint = null;
            agent.CollisionAheadPoint = null;
            // agent.avoi
            return null;
        }
    }
    protected Vector2? GetPositionSteering(Character agent){
        Vector2 direction = agent.Target.position - agent.transform.position.IgnoreZ();
        float distSqrMagnitude = direction.sqrMagnitude;
        if (distSqrMagnitude < agent.targetRadius * agent.targetRadius){
            // Debug.Log("wot");
            return null;
        }

        float targetSpeed;
        if(distSqrMagnitude > agent.slowRadius * agent.slowRadius ){
            targetSpeed = agent.maxSpeed;


        } else {
            // Debug.Log("Slowing");
            targetSpeed = agent.maxSpeed * Mathf.Sqrt(distSqrMagnitude) / agent.slowRadius;
        }
        Vector2 targetVelocity = direction.normalized * targetSpeed;

        return Vector2.ClampMagnitude((targetVelocity - agent.Rb.velocity) / agent.timeToTarget, agent.maxAcceleration);

    }

    protected float? GetRotationSteering(Character agent){
        float rotation = agent.transform.localEulerAngles.z - agent.Target.orientationDeg;
        rotation %= 360;
        // rotation = rotation > 180 ? rotation - 360 : (rotation < -180 ? rotation + 360 : rotation);
        // Debug.Log($"{agent.name}: {(int)agent.transform.localEulerAngles.z }");
        // Debug.Log($"target: {(int)agent.Target.orientationDeg }");
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
        // return new SteeringOutput(GetAvoidanceSteering(agent), GetRotationSteering(agent));
        return new SteeringOutput(GetPositionSteering(agent) + GetAvoidanceSteering(agent) ?? GetPositionSteering(agent) ?? GetAvoidanceSteering(agent) ?? null, GetRotationSteering(agent));
    }

}


public class LeaderSteer : MatchLeaderSteer {

    int currNode = 0;
    public override SteeringOutput GetSteering(Character agent) {
        //path following 
        if(agent.Path != null){
            agent.Target = new PositionOrientation();
            agent.Target.position = agent.Path.nodes[currNode].transform.position;
            if(Vector2.Distance(agent.transform.position, agent.Target.position) <= 2f){
                currNode++;
                if(currNode >= agent.Path.nodes.Count) {
                    currNode = agent.Path.nodes.Count - 1;
                }
            }
        }

        //face target
        Vector2 direction =  agent.transform.position.IgnoreZ() - agent.Target.position;
        if(direction.sqrMagnitude != 0){
            agent.Target.orientationDeg = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg; //trial and error
        }

        return new SteeringOutput(GetPositionSteering(agent) + GetAvoidanceSteering(agent) ?? GetPositionSteering(agent) ?? GetAvoidanceSteering(agent) ?? null, GetRotationSteering(agent));


        // return new SteeringOutput(GetPositionSteering(agent), GetRotationSteering(agent));
        // return new SteeringOutput(GetAvoidanceSteering(agent), GetRotationSteering(agent));
        // Debug.Log($"pos null?: {GetPositionSteering(agent) == null}, obsavoid null?: {GetAvoidanceSteering(agent) == null}");
        // return new SteeringOutput(GetPositionSteering(agent) + GetAvoidanceSteering(agent) ?? GetPositionSteering(agent) ?? GetAvoidanceSteering(agent) ?? null, GetRotationSteering(agent));
    }

}