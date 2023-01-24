//todo better names
public interface IAgentState { 
    public Agent Agent {get; }
    void OnStateEnter();
    void OnStateExit();

}

public abstract class MultiAgentState<TPositionSteer, TRotationSteer> : IAgentState 
    where TPositionSteer : IPositionSteer<ITargetPositionUpdater>
    where TRotationSteer : IRotationSteer
    {
    public Agent Agent {get; protected set; }
    public TPositionSteer PositionSteer {get; protected set; }
    public TRotationSteer RotationSteer {get; protected set; }
    public virtual void OnStateEnter() {}
    public virtual void OnStateExit() {}
}

public class PursueState : MultiAgentState<ArriveSteer, AlignSteer> {
    public PursueState(Agent agent){
        Agent = agent;
        PositionSteer = new ArriveSteer(new LookaheadTargetPositionUpdater());
        RotationSteer =  new AlignSteer(new FaceTargetRotationUpdater());
    }
}
public class FleeState : MultiAgentState<FleeSteer, AlignSteer> {
    public FleeState(Agent agent){
        Agent = agent;
        PositionSteer = new FleeSteer(new LookaheadTargetPositionUpdater());
        RotationSteer =  new AlignSteer(new HideFromTargetRotationUpdater());
    }
}

public class FollowPathState : MultiAgentState<FollowPathSteer, AlignSteer> {
    public FollowPathState(Agent agent){
        Agent = agent;
        PositionSteer = new FollowPathSteer();
        RotationSteer =  new AlignSteer(new LookWhereYoureGoingTargetRotationUpdater());
    }

    public override void OnStateEnter()
    {
        Agent.pathHandler.enabled = true;
        PositionSteer.CurrentParam = 0f;
    }

    public override void OnStateExit()
    {
        Agent.pathHandler.enabled = false;
    }
}

public class SingleAgentState<TSteer> : IAgentState where TSteer : ISteer {
    public Agent Agent {get; protected set; }
    public TSteer Steer {get; protected set; }
    public virtual void OnStateEnter() {}
    public virtual void OnStateExit() {}
}

public class WanderState : SingleAgentState<WanderSteer> {
    public WanderState(Agent agent){
        Agent = agent;
        Steer = new WanderSteer();
    }

    public override void OnStateEnter()
    {
        (Steer.TargetRotationUpdater as WanderTargetUpdater).WanderOrientation = 0f; //todo remove the "as" via generic or polymorph idk
    }

}
