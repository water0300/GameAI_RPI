using System;
using UnityEngine;

public static class SteeringOutputFactory {
    public static SteeringOutput GetSteering(Agent agent, IAgentState steeringContainer){
        switch (steeringContainer) {
            case WanderState c:
                return GetSteering(agent, c.Steer);
            case PursueState c:
                return GetSteering(agent, c.PositionSteer, c.RotationSteer);
            case FleeState c:
                return GetSteering(agent, c.PositionSteer, c.RotationSteer);
            case FollowPathState c:
                return GetSteering(agent, c.PositionSteer, c.RotationSteer);
            default:
                Debug.Log(steeringContainer.GetType());
                throw new Exception(); //todo handle lol
        }
    }
    private static SteeringOutput GetSteering(Agent agent, ISteer steering) => new SteeringOutput(steering.GetPositionSteering(agent), steering.GetRotationSteering(agent));
    private static SteeringOutput GetSteering(Agent agent, IPositionSteer positionSteer, IRotationSteer rotationSteer) => new SteeringOutput(positionSteer.GetPositionSteering(agent), rotationSteer.GetRotationSteering(agent));
}