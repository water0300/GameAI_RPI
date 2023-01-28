using System;
using System.Collections.Generic;

// public interface AgentStateComposite {
//     Agent Agent {get;  }

//     public AgentStateComposite(){
//         var idk = new PursueSubState();
//         idk.PosSteer.
//     }
//     public abstract void OnStateEnter();
//     public abstract void OnStateExit();
// }

public interface ISubState {
    // Agent Agent {get;  }
    abstract void OnStateEnter();
    abstract void OnStateExit();
    abstract SteeringOutput GetSteering();
}

public abstract class SeparableSubState<TPos, TRot> : ISubState where TPos : IPositionSteer where TRot : IRotationSteer  {
    public Agent Agent {get; protected set; }
    abstract public TPos PosSteer {get; protected set;}
    abstract public TRot RotSteer {get; protected set;}
    public SteeringOutput GetSteering() => new SteeringOutput(PosSteer.GetPositionSteering(Agent), RotSteer.GetRotationSteering(Agent));
    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }
}

public abstract class JoinedSubState<T> : ISubState where T : IPositionSteer, IRotationSteer {
    public Agent Agent {get; protected set; }
    abstract public T Steer {get; protected set; }
    public SteeringOutput GetSteering() => new SteeringOutput(Steer.GetPositionSteering(Agent), Steer.GetRotationSteering(Agent));
    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }
}

public class PursueSubState : SeparableSubState<ArriveSteer<LookaheadTargetPositionUpdater>, AlignSteer<FaceTargetRotationUpdater>>{
    override public ArriveSteer<LookaheadTargetPositionUpdater> PosSteer {get; protected set; }
    override public AlignSteer<FaceTargetRotationUpdater> RotSteer {get; protected set; }
    public PursueSubState(Agent agent){
        Agent = agent;
        PosSteer = new ArriveSteer<LookaheadTargetPositionUpdater>();
        RotSteer = new AlignSteer<FaceTargetRotationUpdater>();
    }
}

public class FleeSubState : SeparableSubState<FleeSteer<LookaheadTargetPositionUpdater>, AlignSteer<HideFromTargetRotationUpdater>>{
    override public FleeSteer<LookaheadTargetPositionUpdater> PosSteer {get; protected set; }
    override public AlignSteer<HideFromTargetRotationUpdater> RotSteer {get; protected set; }
    public FleeSubState(Agent agent){
        Agent = agent;
        PosSteer = new FleeSteer<LookaheadTargetPositionUpdater>();
        RotSteer = new AlignSteer<HideFromTargetRotationUpdater>();
    }
}

public class FollowPathSubState : SeparableSubState<FollowPathSteer, AlignSteer<LookWhereYoureGoingTargetRotationUpdater>>{
    override public FollowPathSteer PosSteer {get; protected set; }
    override public AlignSteer<LookWhereYoureGoingTargetRotationUpdater> RotSteer {get; protected set; }
    public FollowPathSubState(Agent agent){
        Agent = agent;
        PosSteer = new FollowPathSteer();
        RotSteer = new AlignSteer<LookWhereYoureGoingTargetRotationUpdater>();
    }
    public override void OnStateEnter(){
        Agent.pathHandler.enabled = true;
        PosSteer.TargetPositionUpdater.CurrentParam = 0f;
    }

    public override void OnStateExit() {
        Agent.pathHandler.enabled = false;
    }
}

public class WanderSubState : JoinedSubState<WanderSteer> {
    override public WanderSteer Steer {get; protected set; }
    public WanderSubState(Agent agent){
        Agent = agent;
        Steer = new WanderSteer();
    }
    public override void OnStateEnter(){
        Steer.TargetRotationUpdater.WanderOrientation = 0f;
    }

}

