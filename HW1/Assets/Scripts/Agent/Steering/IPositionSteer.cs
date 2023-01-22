using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPositionSteer {
    ITargetPositionUpdater TargetPositionUpdater {get; }
    Vector3? GetPositionSteering(Agent agent);
}

public class SeekSteer : IPositionSteer {
    public ITargetPositionUpdater TargetPositionUpdater {get; private set; }
    protected SeekSteer(){}
    public SeekSteer(ITargetPositionUpdater targetPositionUpdater){
        TargetPositionUpdater = targetPositionUpdater;
    }

    public virtual Vector3? GetPositionSteering(Agent agent){
        agent.statusText = "Seeking";
        agent.Target.position = TargetPositionUpdater.GetTargetPosition(agent);
        return GetPositionSteeringHelper(agent);
    }

    protected virtual Vector3? GetPositionSteeringHelper(Agent agent){
        return (agent.Target.position - agent.transform.position).XZPlane().normalized * agent.MaxAcceleration;
    }
}

public class FleeSteer : IPositionSteer {
    public ITargetPositionUpdater TargetPositionUpdater {get; private set; }
    public FleeSteer(ITargetPositionUpdater targetPositionUpdater){
        TargetPositionUpdater = targetPositionUpdater;
    }
    public Vector3? GetPositionSteering(Agent agent){
        agent.statusText = "Fleeing";
        agent.Target.position = TargetPositionUpdater.GetTargetPosition(agent);
        return (agent.transform.position - agent.Target.position).XZPlane().normalized * agent.MaxAcceleration;
    } 

}

public class ArriveSteer : IPositionSteer {
    public ITargetPositionUpdater TargetPositionUpdater {get; private set; }
    public ArriveSteer(ITargetPositionUpdater targetPositionUpdater){
        TargetPositionUpdater = targetPositionUpdater;
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

public class FollowPathSteer : SeekSteer {
    public float CurrentParam {get; private set; } = 0f;
    public FollowPathSteer(){
        
    }
    public override Vector3? GetPositionSteering(Agent agent){

        if(agent.Path == null){
            agent.statusText = "Left click to create path node(s), then Save Path";
            return null;
        } else {
            agent.statusText = "Path Following";
            // CurrentParam;
            //todo at crit point current param stops changing
            CurrentParam = agent.Path.GetParam(agent.transform.position, CurrentParam);
            agent.Target.position = agent.Path.GetTargetPosition(CurrentParam + agent.PathOffset);
            // CurrentParam = CurrentParam + agent.PathOffset;
            return base.GetPositionSteeringHelper(agent);
        }

        
    }
}