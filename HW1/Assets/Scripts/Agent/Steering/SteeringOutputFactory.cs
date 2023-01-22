using System;
public static class SteeringOutputFactory {
    public static SteeringOutput GetSteering(Agent agent, IAgentState steeringContainer){
        switch (steeringContainer) {
            case SingleAgentState c:
                return GetSteering(agent, c.Steer);
            case MultiAgentState c:
                return GetSteering(agent, c.PositionSteer, c.RotationSteer);
            default:
                throw new Exception(); //todo handle lol
        }
    }
    private static SteeringOutput GetSteering(Agent agent, ISteer steering) => new SteeringOutput(steering.GetPositionSteering(agent), steering.GetRotationSteering(agent));
    private static SteeringOutput GetSteering(Agent agent, IPositionSteer positionSteer, IRotationSteer rotationSteer) => new SteeringOutput(positionSteer.GetPositionSteering(agent), rotationSteer.GetRotationSteering(agent));
}