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
    
    void OnUpdate(Agent agent, SteeringOutput steering, float time);
    SteeringOutput GetSteering(Agent agent);

}

public abstract class MoveState : IAgentState {
    public void OnUpdate(Agent agent, SteeringOutput steering, float time){
        agent.Rb.MovePosition(agent.Rb.position + agent.Velocity * time);
        agent.Rb.MoveRotation(Quaternion.Euler(agent.Rb.rotation.eulerAngles + agent.AngularVelocity.eulerAngles * time));

        agent.Velocity = Vector3.ClampMagnitude(agent.Velocity + steering.linearAcceleration*time, agent.maxSpeed);
        agent.AngularVelocity = Quaternion.Euler(agent.AngularVelocity.eulerAngles + steering.angularAcceleration.eulerAngles * time);
    }
    public abstract SteeringOutput GetSteering(Agent agent);

}

public class SeekState : MoveState {
    public override SteeringOutput GetSteering(Agent agent) => new SteeringOutput((agent.Target.transform.position - agent.transform.position).XZPlane().normalized * agent.accelerationFactor, Quaternion.identity);

}

public class FleeState : MoveState {
    public override SteeringOutput GetSteering(Agent agent) => new SteeringOutput((agent.transform.position - agent.Target.transform.position).XZPlane().normalized * agent.accelerationFactor, Quaternion.identity);

}