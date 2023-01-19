using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentState {
    public Agent Agent {get; private set; }
    public ITargetUpdater TargetUpdater {get; private set; }
    public AgentState(Agent agent){
        Agent = agent;
        TargetUpdater = new DirectTargetPositionUpdater();
    }
    public AgentState(Agent agent, ITargetUpdater targetPositionUpdater){
        Agent = agent;
        TargetUpdater = targetPositionUpdater;
    }

    public abstract SteeringOutput? GetSteering();

}

public class SeekState : AgentState {
    public SeekState(Agent agent) : base(agent) {}
    public SeekState(Agent agent, ITargetUpdater targetPositionUpdater) : base(agent, targetPositionUpdater) {}

    public override SteeringOutput? GetSteering(){
        Agent.Target.position = TargetUpdater.GetTargetPosition(Agent);
        return new SteeringOutput((Agent.Target.position - Agent.transform.position).XZPlane().normalized * Agent.maxAcceleration, 0f);
    } 

}

public class FleeState : AgentState {
    public FleeState(Agent agent) : base(agent) {}
    public FleeState(Agent agent, ITargetUpdater targetPositionUpdater) : base(agent, targetPositionUpdater) {}

    public override SteeringOutput? GetSteering(){
        Agent.Target.position = TargetUpdater.GetTargetPosition(Agent);
        return new SteeringOutput((Agent.transform.position - Agent.Target.position).XZPlane().normalized * Agent.maxAcceleration, 0f);
    } 

}

public class ArriveState : AgentState {
    public ArriveState(Agent agent) : base(agent) {}
    public ArriveState(Agent agent, ITargetUpdater targetPositionUpdater) : base(agent, targetPositionUpdater) {}
    public override SteeringOutput? GetSteering(){
        Agent.Target.position = TargetUpdater.GetTargetPosition(Agent);

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
    public AlignState(Agent agent) : base(agent)
    {
    }
    public AlignState(Agent agent, ITargetUpdater targetPositionUpdater) : base(agent, targetPositionUpdater)
    {
    }
    public override SteeringOutput? GetSteering()
    {
        Agent.Target.rotation = TargetUpdater.GetTargetRotation(Agent);

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

