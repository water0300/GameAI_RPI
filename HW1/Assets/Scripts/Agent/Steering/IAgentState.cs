//todo better names
public interface IAgentState{ }

public abstract class MultiAgentState : IAgentState {
    public IPositionSteer PositionSteer {get; protected set; }
    public IRotationSteer RotationSteer {get; protected set; }
}

public class PursueState : MultiAgentState {
    public PursueState(){
        PositionSteer = new ArriveSteer(new LookaheadTargetPositionUpdater());
        RotationSteer =  new AlignSteer(new FaceTargetRotationUpdater());
    }
}
public class FleeState : MultiAgentState {
    public FleeState(){
        PositionSteer = new FleeSteer(new LookaheadTargetPositionUpdater());
        RotationSteer =  new AlignSteer(new HideFromTargetRotationUpdater());
    }
}

public class FollowPathState : MultiAgentState {
    public FollowPathState(){
        PositionSteer = new FollowPathSteer();
        RotationSteer =  new AlignSteer(new LookWhereYoureGoingTargetRotationUpdater());
    }
}

public class SingleAgentState : IAgentState {
    public ISteer Steer {get; protected set; }
}

public class WanderState : SingleAgentState {
    public WanderState(){
        Steer = new WanderSteer();
    }
}
