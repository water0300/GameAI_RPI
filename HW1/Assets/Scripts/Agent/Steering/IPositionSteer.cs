using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPositionSteer {
    Vector3? GetPositionSteering(Agent agent);
}

public interface IPositionSteer<T> : IPositionSteer where T : ITargetPositionUpdater {
    public T TargetPositionUpdater {get; }
}

public class SeekSteer<T> : IPositionSteer<T> where T : ITargetPositionUpdater, new(){
    public T TargetPositionUpdater {get; protected set; }
    public SeekSteer(){
        TargetPositionUpdater = new T();
    }

    public virtual Vector3? GetPositionSteering(Agent agent){
        agent.statusText = "Seeking";
        TargetPositionUpdater.UpdateTargetPosition(agent);
        return GetSeekSteering(agent);
    }

    protected Vector3? GetSeekSteering(Agent agent){
        return (agent.Target.position - agent.transform.position).XZPlane().normalized * agent.MaxAcceleration;
    }
}

public class FleeSteer<T> : IPositionSteer<T> where T : ITargetPositionUpdater, new() {
    public T TargetPositionUpdater {get; private set; }
    public FleeSteer(){
        TargetPositionUpdater = new T();
    }
    public Vector3? GetPositionSteering(Agent agent){
        agent.statusText = "Fleeing";
        TargetPositionUpdater.UpdateTargetPosition(agent);
        return (agent.transform.position - agent.Target.position).XZPlane().normalized * agent.MaxAcceleration;
    } 

}

public class ArriveSteer<T> : IPositionSteer<T> where T : ITargetPositionUpdater, new(){
    public T TargetPositionUpdater {get; private set; }
    public ArriveSteer(){
        TargetPositionUpdater = new T();
    }
    public Vector3? GetPositionSteering(Agent agent){
        TargetPositionUpdater.UpdateTargetPosition(agent);

        Vector3 direction = agent.Target.position - agent.transform.position;
        float distSqrMagnitude = direction.sqrMagnitude;
        if (distSqrMagnitude < agent.TargetRadius * agent.TargetRadius){
            agent.statusText = "Idle";
            return null;
        }

        float targetSpeed;
        if(distSqrMagnitude > agent.SlowRadius * agent.SlowRadius ){
            targetSpeed = agent.MaxSpeed;
            agent.statusText = "Seeking";


        } else {
            targetSpeed = agent.MaxSpeed * Mathf.Sqrt(distSqrMagnitude) / agent.SlowRadius;
            agent.statusText = "Arriving";
        }
        Vector3 targetVelocity = direction.XZPlane().normalized * targetSpeed;

        return Vector3.ClampMagnitude((targetVelocity - agent.Velocity) / agent.TimeToTarget, agent.MaxAcceleration);

    }

}

public class FollowPathSteer : SeekSteer<FollowPathTargetPositionUpdater> {
    public FollowPathSteer(){ 
        TargetPositionUpdater = new FollowPathTargetPositionUpdater();
    }
    public override Vector3? GetPositionSteering(Agent agent){
        // Debug.Log(agent.Path == null);
        if(agent.Path == null){
            agent.statusText = "Left click to create path node(s), then Save Path";
            return null;
        } else {
            agent.statusText = "Path Following";
            TargetPositionUpdater.UpdateTargetPosition(agent);
            return base.GetSeekSteering(agent);
        }

        
    }
}

public class SeparationSteer : IPositionSteer {
    public Vector3 ClosestObstaclePos {get; protected set; }
    public float LorRMod {get; protected set; }
    public SeparationSteer(){}
    public virtual Vector3? GetPositionSteering(Agent agent) {
       return null;
    }

    protected virtual Vector3? GetSeparationSteering(Agent agent){
        Vector3 direction = (agent.transform.position - ClosestObstaclePos).XZPlane();

        // if(direction.sqrMagnitude < agent.Threshold * agent.Threshold){
            float strength = Mathf.Min(agent.DecayCoefficient / direction.sqrMagnitude, agent.MaxAcceleration);
            Vector3 tangent = Vector3.Cross(direction.normalized, Vector3.up) * LorRMod; //respect to y axis
            return direction * strength + strength * tangent;
        // } else {
        //     return null;
        // }
    }

}

public class ConeCheckSteer : SeparationSteer {
    public override Vector3? GetPositionSteering(Agent agent){
       Collider[] cols = Physics.OverlapSphere(agent.transform.position, agent.Threshold, ~(1 << 6 | 1 << 7) );
        if(cols.Length == 0) {
            return null;
        }

        Vector3 closestPos = Vector3.zero;
        foreach(Collider col in cols){
            if(closestPos == Vector3.zero || Vector3.Distance(col.transform.position, agent.transform.position) < Vector3.Distance(closestPos, agent.transform.position)){
                Vector3 direction = (col.transform.position - agent.transform.position).XZPlane();
                // Debug.Log(Vector3.Dot(agent.transform.rotation.AsNormVector(), direction.normalized));
                if(Vector3.Dot(agent.transform.rotation.AsNormVector(), direction.normalized) > agent.ConeThreshold){
                    Debug.Log("wot");
                    closestPos = col.transform.position;
                    var dot = col.bounds.center.x * -col.bounds.ClosestPoint(agent.transform.position).z +  -col.bounds.center.z * col.bounds.ClosestPoint(agent.transform.position).x;
                    if(dot > 0){ //closest point is right of center
                        LorRMod = 1;
                        // Debug.Log("right of center");
                    } else {
                        LorRMod = -1;
                        // Debug.Log("left of center");

                    }
                }
            }
        }

        if(closestPos == Vector3.zero){
            return null;
        } else {
            ClosestObstaclePos = closestPos;
            return base.GetSeparationSteering(agent);
        }
    }

}

public class RayCastSteer : SeparationSteer {
    public override Vector3? GetPositionSteering(Agent agent){
        RaycastHit hit;
        if(Physics.Raycast(agent.transform.position, -agent.transform.right, out hit, agent.Threshold, ~(1 << 6 | 1 << 7))){
            Vector3 direction = (hit.transform.position - agent.transform.position).XZPlane();
            ClosestObstaclePos = hit.transform.position;
            var dot = hit.collider.bounds.center.x * -hit.collider.bounds.ClosestPoint(agent.transform.position).z +  -hit.collider.bounds.center.z * hit.collider.bounds.ClosestPoint(agent.transform.position).x;
            if(dot > 0){ //closest point is right of center
                LorRMod = 1;
                Debug.Log("right of center");
            } else {
                LorRMod = -1;
                Debug.Log("left of center");

            }
            return base.GetSeparationSteering(agent);
        } else {
            return null;
        }
    }
}

public class ObstacleAvoidanceSteer : SeparationSteer {
    public override Vector3? GetPositionSteering(Agent agent){
        Collider[] cols = Physics.OverlapSphere(agent.transform.position, agent.Threshold, ~(1 << 6 | 1 << 7) );
        if(cols.Length == 0) {
            return null;
        }

        float shortestTime = Mathf.Infinity;
        Collider firstTarget = null;
        float firstMinSeparation = 0f;
        float firstDistance = 0f;
        Vector3 firstRelativePos = Vector3.zero;
        Vector3 firstRelativeVel = Vector3.zero;
        foreach(Collider col in cols){
            Vector3 relativePos = col.transform.position - agent.transform.position;
            Rigidbody colRb;
            Vector3 relativeVel;
            if(col.TryGetComponent<Rigidbody>(out colRb)){
                relativeVel = colRb.velocity - agent.Rb.velocity;
            } else {
                relativeVel = -agent.Rb.velocity;
            }
            float relativeSpeed = relativeVel.magnitude;
            float ttCollision = Vector3.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);
            // Debug.Log(ttCollision);

            float distance = relativePos.magnitude;
            float minSeparation = distance - relativeSpeed * ttCollision;
            if(minSeparation > 2 * agent.CollisionRadius){
                continue;
            }

            if(ttCollision > 0 && ttCollision < shortestTime){
                shortestTime = ttCollision;
                firstTarget = col;
                firstMinSeparation = minSeparation;
                firstDistance = distance;
                firstRelativePos = relativePos;
                firstRelativeVel = relativeVel;
            }
        

            if(firstTarget == null){
                return null;
            }

            if(firstMinSeparation <= 0 || firstDistance < 2 * agent.CollisionRadius){
                relativePos = firstTarget.transform.position - agent.transform.position;
            } else {
                relativePos = firstRelativePos + firstRelativeVel * shortestTime;
            }
            relativePos.Normalize();
            return relativePos * agent.MaxAcceleration;
                
        }
        return null;



    }
}