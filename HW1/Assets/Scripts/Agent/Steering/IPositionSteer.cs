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
        agent.Target.position = TargetPositionUpdater.GetTargetPosition(agent);
        return GetSeekSteering(agent);
    }

    protected virtual Vector3? GetSeekSteering(Agent agent){
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
        agent.Target.position = TargetPositionUpdater.GetTargetPosition(agent);
        return (agent.transform.position - agent.Target.position).XZPlane().normalized * agent.MaxAcceleration;
    } 

}

public class ArriveSteer<T> : IPositionSteer<T> where T : ITargetPositionUpdater, new(){
    public T TargetPositionUpdater {get; private set; }
    public ArriveSteer(){
        TargetPositionUpdater = new T();
    }
    public Vector3? GetPositionSteering(Agent agent){
        agent.Target.position = TargetPositionUpdater.GetTargetPosition(agent);

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
        if(agent.Path == null){
            agent.statusText = "Left click to create path node(s), then Save Path";
            return null;
        } else {
            agent.statusText = "Path Following";
            agent.Target.position = TargetPositionUpdater.GetTargetPosition(agent);
            return base.GetSeekSteering(agent);
        }

        
    }
}

public class ConeCheckSteer : IPositionSteer {
    public Vector3? GetPositionSteering(Agent agent){
        throw new NotImplementedException();
    }
}