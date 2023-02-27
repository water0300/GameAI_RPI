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

public class Steering {
    protected Vector2? GetAvoidanceSteering(Agent agent){
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
    protected Vector2? GetPositionSteering(Agent agent){
        Vector2 direction = agent.Target.position - agent.transform.position.IgnoreZ();
        float distSqrMagnitude = direction.sqrMagnitude;
        if (distSqrMagnitude < agent.targetRadius * agent.targetRadius){
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

    protected float? GetRotationSteering(Agent agent){
        float rotation = agent.transform.eulerAngles.z - agent.Target.orientationDeg;
        rotation %= 360;
        rotation = rotation > 180 ? rotation - 360 : (rotation < -180 ? rotation + 360 : rotation);
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

    public int currNode = 0;
    public SteeringOutput GetSteering(Agent agent) {

        return(new SteeringOutput(GetAvoidanceSteering(agent) ?? null, null));

        //path following 
        // if(agent.Path != null && agent.Path.Count > 0 && currNode < agent.Path.Count){
        //     agent.Target = new PositionOrientation();
        //     // Debug.Log(agent.Path.Count);
        //     agent.Target.position = agent.Path[currNode].transform.position;
        //     if(Vector2.Distance(agent.transform.position, agent.Target.position) <= 1.5f){
        //         currNode++;
        //         if(currNode >= agent.Path.Count) {
        //             // currNode = agent.Path.Count - 1;
        //             return new SteeringOutput(null, null);
        //         }
        //     }
        //     //look hwere you're going
        //     if(agent.Rb.velocity.sqrMagnitude != 0){
        //         agent.Target.orientationDeg = Mathf.Atan2(-agent.Rb.velocity.x, agent.Rb.velocity.y) * Mathf.Rad2Deg; //trial and error
        //     } 

        //     return new SteeringOutput(GetPositionSteering(agent) + GetAvoidanceSteering(agent) ?? GetPositionSteering(agent) ?? GetAvoidanceSteering(agent) ?? null, GetRotationSteering(agent));

        // } else{
        //     return new SteeringOutput(null, null);
        // }



    }

}

