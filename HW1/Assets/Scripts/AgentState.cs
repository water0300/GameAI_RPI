using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPositionSteer {
    ITargetPositionUpdater TargetPositionUpdater {get; }
    Vector3? GetPositionSteering(Agent agent);
}

public interface IRotationSteer{
    ITargetRotationUpdater TargetRotationUpdater {get; }
    float? GetRotationSteering(Agent agent);
}

public interface ISteer : IPositionSteer, IRotationSteer {}

public static class AgentStateFactory {
    public static SteeringOutput GetSteering(Agent agent, ISteer steering) => new SteeringOutput(steering.GetPositionSteering(agent), steering.GetRotationSteering(agent));
    public static SteeringOutput GetSteering(Agent agent, IPositionSteer positionSteer, IRotationSteer rotationSteer) => new SteeringOutput(positionSteer.GetPositionSteering(agent), rotationSteer.GetRotationSteering(agent));


}

public class SeekSteer : IPositionSteer {
    public ITargetPositionUpdater TargetPositionUpdater {get; private set; }
    protected SeekSteer(){}
    public SeekSteer(ITargetPositionUpdater targetPositionUpdater){
        TargetPositionUpdater = targetPositionUpdater;
    }

    public virtual Vector3? GetPositionSteering(Agent agent){
        agent.Target.position = TargetPositionUpdater.GetTargetPosition(agent);
        return GetPositionSteeringHelper(agent);
    }

    protected virtual Vector3? GetPositionSteeringHelper(Agent agent){
        return (agent.Target.position - agent.transform.position).XZPlane().normalized * agent.maxAcceleration;
    }
}

public class FleeSteer : IPositionSteer {
    public ITargetPositionUpdater TargetPositionUpdater {get; private set; }
    public FleeSteer(ITargetPositionUpdater targetPositionUpdater){
        TargetPositionUpdater = targetPositionUpdater;
    }
    public Vector3? GetPositionSteering(Agent agent){
        agent.Target.position = TargetPositionUpdater.GetTargetPosition(agent);
        return (agent.transform.position - agent.Target.position).XZPlane().normalized * agent.maxAcceleration;
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
        if (distSqrMagnitude < agent.targetRadius * agent.targetRadius){
            // Debug.Log("Arriving");
            return null;
        }

        float targetSpeed;
        if(distSqrMagnitude > agent.slowRadius * agent.slowRadius ){
            targetSpeed = agent.maxSpeed;
            // Debug.Log($"Full Steam: {targetSpeed}");

        } else {
            targetSpeed = agent.maxSpeed * Mathf.Sqrt(distSqrMagnitude) / agent.slowRadius;
            // Debug.Log($"Slowing: {targetSpeed}");
        }
        Vector3 targetVelocity = direction.XZPlane().normalized * targetSpeed;

        return Vector3.ClampMagnitude((targetVelocity - agent.Velocity) / agent.timeToTarget, agent.maxAcceleration);

    }

}

public class AlignSteer : IRotationSteer {
    public ITargetRotationUpdater TargetRotationUpdater {get; protected set; }
    protected AlignSteer() {}
    public AlignSteer(ITargetRotationUpdater targetRotationUpdater){
        TargetRotationUpdater = targetRotationUpdater;
    }
    public virtual float? GetRotationSteering(Agent agent) {
        agent.Target.rotation = TargetRotationUpdater.GetTargetRotation(agent);
        return GetAlignSteering(agent);
    }

    protected float? GetAlignSteering(Agent agent){
        float rotation = agent.transform.rotation.eulerAngles.y - agent.Target.transform.rotation.eulerAngles.y;
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
        return Mathf.Clamp((targetAngularSpeed_Y - agent.AngularSpeed_Y) / agent.timeToAlign, -agent.maxAngularAcceleration_Y, agent.maxAngularAcceleration_Y);
    }
}

public class WanderSteer : AlignSteer, ISteer {
    public ITargetPositionUpdater TargetPositionUpdater { get; private set; }

    public WanderSteer() {
        WanderTargetUpdater wtu = new WanderTargetUpdater();
        TargetRotationUpdater = wtu;
        TargetPositionUpdater = wtu;
    }

    private Vector3 GetWanderTargetPosition(Agent agent){
        return (agent.transform.position + agent.wanderOffset * agent.transform.rotation.AsNormVector()) + agent.wanderRadius * agent.Target.rotation.AsNormVector();
    }

    public override float? GetRotationSteering(Agent agent){
        agent.Target.rotation = TargetRotationUpdater.GetTargetRotation(agent);
        agent.Target.position = TargetPositionUpdater.GetTargetPosition(agent);
        return GetAlignSteering(agent);
    }
    public Vector3? GetPositionSteering(Agent agent) => agent.maxAcceleration * agent.transform.rotation.AsNormVector();
    

}

public class FollowPathSteer : SeekSteer {
    public float CurrentParam {get; private set; } = 0f;
    public FollowPathSteer(){
        
    }
    public override Vector3? GetPositionSteering(Agent agent){
        if(agent.Path == null){
            return null;
        }
        CurrentParam = agent.currParam;
        //todo at crit point current param stops changing
        // CurrentParam = agent.Path.GetParam(agent.transform.position);
        agent.Target.position = agent.Path.GetTargetPosition(CurrentParam + agent.pathOffset);
        return base.GetPositionSteeringHelper(agent);
    }
}