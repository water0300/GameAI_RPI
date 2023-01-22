using System;
public static class SteeringOutputFactory {
    public static SteeringOutput GetSteering(Agent agent, ISteerContainer steeringContainer){
        switch (steeringContainer) {
            case SingleSteerContainer c:
                return GetSteering(agent, c.Steer);
            case MultiSteerContainer c:
                return GetSteering(agent, c.PositionSteer, c.RotationSteer);
            default:
                throw new Exception(); //todo handle lol
        }
    }
    private static SteeringOutput GetSteering(Agent agent, ISteer steering) => new SteeringOutput(steering.GetPositionSteering(agent), steering.GetRotationSteering(agent));
    private static SteeringOutput GetSteering(Agent agent, IPositionSteer positionSteer, IRotationSteer rotationSteer) => new SteeringOutput(positionSteer.GetPositionSteering(agent), rotationSteer.GetRotationSteering(agent));
}