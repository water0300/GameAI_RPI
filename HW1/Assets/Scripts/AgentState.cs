using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentState {
    public Agent Agent {get; private set; }
    public ITargetPositionUpdater TargetPositionUpdater {get; protected set; }
    public ITargetRotationUpdater TargetRotationUpdater {get; protected set; }

    public AgentState(Agent agent){
        Agent = agent;
    }
    public AgentState(Agent agent, ITargetPositionUpdater targetPositionUpdater){
        Agent = agent;
        TargetPositionUpdater = targetPositionUpdater;
    }    
    public AgentState(Agent agent, ITargetRotationUpdater targetRotationUpdater){
        Agent = agent;
        TargetRotationUpdater = targetRotationUpdater;
    }
    public AgentState(Agent agent, ITargetPositionUpdater targetPositionUpdater, ITargetRotationUpdater targetRotationUpdater){
        Agent = agent;
        TargetPositionUpdater = targetPositionUpdater;
        TargetRotationUpdater = targetRotationUpdater;
    }
    public abstract SteeringOutput? GetSteering();

}

public class SeekState : AgentState {
    public SeekState(Agent agent) : base(agent) {
        TargetPositionUpdater = new DirectTargetPositionUpdater();
    }
    public SeekState(Agent agent, ITargetPositionUpdater targetPositionUpdater) : base(agent, targetPositionUpdater) {}

    public override SteeringOutput? GetSteering(){
        Agent.Target.position = TargetPositionUpdater.GetTargetPosition(Agent);
        return new SteeringOutput((Agent.Target.position - Agent.transform.position).XZPlane().normalized * Agent.maxAcceleration, 0f);
    } 

}

public class FleeState : AgentState {
    public FleeState(Agent agent) : base(agent) {
        TargetPositionUpdater = new DirectTargetPositionUpdater();
    }
    public FleeState(Agent agent, ITargetPositionUpdater targetPositionUpdater) : base(agent, targetPositionUpdater) {}

    public override SteeringOutput? GetSteering(){
        Agent.Target.position = TargetPositionUpdater.GetTargetPosition(Agent);
        return new SteeringOutput((Agent.transform.position - Agent.Target.position).XZPlane().normalized * Agent.maxAcceleration, 0f);
    } 

}

public class ArriveState : AgentState {
    public ArriveState(Agent agent) : base(agent) {
        TargetPositionUpdater = new DirectTargetPositionUpdater();
    }
    public ArriveState(Agent agent, ITargetPositionUpdater targetPositionUpdater) : base(agent, targetPositionUpdater) {}
    public override SteeringOutput? GetSteering(){
        Agent.Target.position = TargetPositionUpdater.GetTargetPosition(Agent);

        Vector3 direction = Agent.Target.position - Agent.transform.position;
        float distSqrMagnitude = direction.sqrMagnitude;
        if (distSqrMagnitude < Agent.targetRadius * Agent.targetRadius){
            // Debug.Log("Arriving");
            return null;
        }

        float targetSpeed;
        if(distSqrMagnitude > Agent.slowRadius * Agent.slowRadius ){
            targetSpeed = Agent.maxSpeed;
            // Debug.Log($"Full Steam: {targetSpeed}");

        } else {
            targetSpeed = Agent.maxSpeed * Mathf.Sqrt(distSqrMagnitude) / Agent.slowRadius;
            // Debug.Log($"Slowing: {targetSpeed}");
        }
        Vector3 targetVelocity = direction.XZPlane().normalized * targetSpeed;

        return new SteeringOutput(Vector3.ClampMagnitude((targetVelocity - Agent.Velocity) / Agent.timeToTarget, Agent.maxAcceleration), 0f);

    }

}

public class AlignState : AgentState
{
    public AlignState(Agent agent) : base(agent) {
        TargetRotationUpdater = new FaceTargetRotationUpdater();
    }
    public AlignState(Agent agent, ITargetRotationUpdater targetRotationUpdater) : base(agent, targetRotationUpdater) {}
    
    protected virtual Quaternion GetTargetRotation() => TargetRotationUpdater.GetTargetRotation(Agent);
    protected virtual Vector3? GetTargetPosition() => null;
    public override SteeringOutput? GetSteering()
    {
        Agent.Target.rotation = GetTargetRotation();
        Agent.Target.position = GetTargetPosition() ?? Agent.Target.position;
        float rotation = Agent.transform.rotation.eulerAngles.y - Agent.Target.transform.rotation.eulerAngles.y;
        rotation %= 360;
        rotation = rotation > 180 ? rotation - 360 : (rotation < -180 ? rotation + 360 : rotation);
        float rotationSize = Mathf.Abs(rotation);
        if(rotationSize < Agent.targetAlignWindow){
            return null;
        }
        // Debug.Log($"Rotation: {rotation}");

        float targetAngularSpeed_Y;
        if(rotationSize > Agent.slowAlignWindow){
            targetAngularSpeed_Y = Agent.maxAngularSpeed_Y; 
        } else {
            targetAngularSpeed_Y = Agent.maxAngularSpeed_Y * rotationSize / Agent.slowAlignWindow;
        }
        // Debug.Log($"TargetRotation: {targetRotation}");

        targetAngularSpeed_Y *= rotation/rotationSize;



        float angular = Mathf.Clamp((targetAngularSpeed_Y - Agent.AngularSpeed_Y) / Agent.timeToAlign, -Agent.maxAngularAcceleration_Y, Agent.maxAngularAcceleration_Y);
        // Debug.Log($"math: {targetAngularSpeed_Y - Agent.AngularSpeed_Y}");

        return new SteeringOutput(Vector3.zero, angular);
    }
}

public class WanderState : AlignState {
    public float WanderOrientation {get; private set; } = 0f;
    public WanderState(Agent agent) : base(agent) {}

    protected override Quaternion GetTargetRotation(){
        WanderOrientation += Utilities.RandomBinomial() * Agent.wanderRate;
        return Quaternion.AngleAxis(WanderOrientation + Agent.transform.rotation.eulerAngles.y, Vector3.down);
    }

    protected override Vector3? GetTargetPosition(){
        return (Agent.transform.position + Agent.wanderOffset * Agent.transform.rotation.AsVector()) + Agent.wanderRadius * Agent.Target.rotation.AsVector();
    }

    public override SteeringOutput? GetSteering(){
        return new SteeringOutput(Agent.maxAcceleration * Agent.transform.rotation.AsVector(), base.GetSteering()?.angularAcceleration ?? 0f);
        
    }

}

