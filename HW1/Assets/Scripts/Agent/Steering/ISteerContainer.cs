//todo better names
public interface ISteerContainer{ }

public class MultiSteerContainer : ISteerContainer {
    public IPositionSteer PositionSteer {get; private set; }
    public IRotationSteer RotationSteer {get; private set; }
    public MultiSteerContainer(IPositionSteer positionSteer, IRotationSteer rotationSteer){
        PositionSteer = positionSteer;
        RotationSteer = rotationSteer;
    }
}
   
public class SingleSteerContainer : ISteerContainer {
    public ISteer Steer {get; private set; }
    public SingleSteerContainer(ISteer steer){
        Steer = steer;
    }
}