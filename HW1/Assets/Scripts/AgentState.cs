using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPositionSteer {
    ITargetPositionUpdater TargetPositionUpdater {get; set; }
    Vector3? GetPositionSteering(Agent agent);
}

public interface IRotationSteer{
    ITargetRotationUpdater TargetRotationUpdater {get; set; }
    float? GetRotationSteering(Agent agent);
}

public interface ISteer : IPositionSteer, IRotationSteer {}

public static class AgentStateFactory {
    public static SteeringOutput GetSteering(Agent agent, ISteer steering) => new SteeringOutput(steering.GetPositionSteering(agent), steering.GetRotationSteering(agent));
    public static SteeringOutput GetSteering(Agent agent, IPositionSteer positionSteer, IRotationSteer rotationSteer) => new SteeringOutput(positionSteer.GetPositionSteering(agent), rotationSteer.GetRotationSteering(agent));


}

public class SeekSteer : IPositionSteer {
    public ITargetPositionUpdater TargetPositionUpdater {get; set; }
    public SeekSteer(ITargetPositionUpdater targetPositionUpdater){
        TargetPositionUpdater = targetPositionUpdater;
    }

    public Vector3? GetPositionSteering(Agent agent){
        agent.Target.position = TargetPositionUpdater.GetTargetPosition(agent);
        return (agent.Target.position - agent.transform.position).XZPlane().normalized * agent.maxAcceleration;
    }
}

public class FleeSteer : IPositionSteer {
    public ITargetPositionUpdater TargetPositionUpdater {get; set; }
    public FleeSteer(ITargetPositionUpdater targetPositionUpdater){
        TargetPositionUpdater = targetPositionUpdater;
    }
    public Vector3? GetPositionSteering(Agent agent){
        agent.Target.position = TargetPositionUpdater.GetTargetPosition(agent);
        return (agent.transform.position - agent.Target.position).XZPlane().normalized * agent.maxAcceleration;
    } 

}

public class ArriveState : IPositionSteer {
    public ITargetPositionUpdater TargetPositionUpdater {get; set; }
    public ArriveState(ITargetPositionUpdater targetPositionUpdater){
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

public class AlignState : IRotationSteer {
    public ITargetRotationUpdater TargetRotationUpdater {get; set; }
    protected AlignState() {}
    public AlignState(ITargetRotationUpdater targetRotationUpdater){
        TargetRotationUpdater = targetRotationUpdater;
    }
    public virtual float? GetRotationSteering(Agent agent) {
        agent.Target.rotation = TargetRotationUpdater.GetTargetRotation(agent);
        return GetRotationSteeringHelper(agent);
    }

    protected float? GetRotationSteeringHelper(Agent agent){
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

public class WanderState : AlignState, ISteer {
    public WanderState() {}
    private WanderState(ITargetRotationUpdater targetRotationUpdater) : base(targetRotationUpdater) {}

    public float WanderOrientation {get; private set; } = 0f;
    public ITargetPositionUpdater TargetPositionUpdater {get; set; }

    protected Quaternion GetWanderTargetRotation(Agent agent){
        WanderOrientation += Utilities.RandomBinomial() * agent.wanderRate; //todo smoothdamp?
        // WanderOrientation = Mathf.SmoothDampAngle(WanderOrientation, Utilities.RandomBinomial() * agent.wanderRate, ref v, 0.1f); //todo smoothdamp?
        Debug.Log($"Wander: {WanderOrientation}");
        return Quaternion.AngleAxis(WanderOrientation + agent.transform.rotation.eulerAngles.y, Vector3.up);
    }

    private Vector3 GetWanderTargetPosition(Agent agent){
        return (agent.transform.position + agent.wanderOffset * agent.transform.rotation.AsVector()) + agent.wanderRadius * agent.Target.rotation.AsVector();
    }

    public override float? GetRotationSteering(Agent agent){
        agent.Target.rotation = GetWanderTargetRotation(agent);
        agent.Target.position = GetWanderTargetPosition(agent);
        return GetRotationSteeringHelper(agent);
    }
    public Vector3? GetPositionSteering(Agent agent) => agent.maxAcceleration * agent.transform.rotation.AsVector();
    

}

