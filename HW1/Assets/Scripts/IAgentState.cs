using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public struct SteeringOutput {
    public Vector3 linearAcceleration;
    public Quaternion angularAcceleration;
    public SteeringOutput(Vector3 linear, Quaternion angular){
        this.linearAcceleration = linear;
        this.angularAcceleration = angular;
    }

}
public interface IAgentState {
    
    void OnUpdate(Agent agent, float time);
    Nullable<SteeringOutput> GetSteering(Agent agent);

}

public abstract class MoveState : IAgentState {
    public void OnUpdate(Agent agent, float time){
        Nullable<SteeringOutput> steering = GetSteering(agent);

        agent.Rb.MovePosition(agent.Rb.position + agent.Velocity * time);
        agent.Rb.MoveRotation(Quaternion.Euler(agent.Rb.rotation.eulerAngles + agent.AngularVelocity.eulerAngles * time));
        agent.Velocity = Vector3.ClampMagnitude(agent.Velocity + steering?.linearAcceleration*time ?? Vector3.zero, agent.maxSpeed);
        agent.AngularVelocity = Quaternion.Euler(agent.AngularVelocity.eulerAngles + steering?.angularAcceleration.eulerAngles * time ?? Vector3.zero);
        

        
    }
    public abstract Nullable<SteeringOutput> GetSteering(Agent agent);

}

public class SeekAndArriveState : MoveState {
    public virtual Vector3 GetTargetPosition(Agent agent) => agent.TargetRB.position;
    
    public override Nullable<SteeringOutput> GetSteering(Agent agent){
        agent.Target.position = GetTargetPosition(agent);

        Vector3 direction = agent.Target.position - agent.transform.position;
        float distSqrMagnitude = direction.sqrMagnitude;
        if (distSqrMagnitude < agent.arrivalRadius * agent.arrivalRadius){
            // Debug.Log("Arriving");
            return null;
        }

        float targetSpeed;
        if(distSqrMagnitude > agent.slowdownRadius * agent.slowdownRadius ){
            targetSpeed = agent.maxSpeed;
            // Debug.Log($"Full Steam: {targetSpeed}");

        } else {
            targetSpeed = agent.maxSpeed * Mathf.Sqrt(distSqrMagnitude) / agent.slowdownRadius;
            // Debug.Log($"Slowing: {targetSpeed}");
        }
        Vector3 targetVelocity = direction.XZPlane().normalized * targetSpeed;

        return new SteeringOutput(Vector3.ClampMagnitude((targetVelocity - agent.Velocity) / agent.timeToTarget, agent.maxAcceleration), Quaternion.identity);

    }

}

public class PursueState : SeekAndArriveState {
    public override Vector3 GetTargetPosition(Agent agent){
        Vector3 direction = agent.Target.position - agent.transform.position;
        float distSqrMagnitude = direction.sqrMagnitude;
        float speedSqrMagnitude = agent.Velocity.sqrMagnitude;
        float prediction;
        if (speedSqrMagnitude <= distSqrMagnitude / (agent.maxPrediction * agent.maxPrediction)){
            prediction = agent.maxPrediction;
        } else {
            prediction = distSqrMagnitude / speedSqrMagnitude;
        }

        return agent.TargetRB.position + agent.TargetRB.velocity * prediction;
    }

}

public class FleeState : MoveState {
    public override Nullable<SteeringOutput> GetSteering(Agent agent) => new SteeringOutput(Vector3.ClampMagnitude((agent.transform.position - agent.Target.transform.position).XZPlane(), agent.maxAcceleration), Quaternion.identity);

}