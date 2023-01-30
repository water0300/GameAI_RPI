using UnityEngine;
using System.Collections.Generic;

public class AgentStateComposite {
    public Agent Agent {get; private set;}
    public Dictionary<ISubState, float> BehaviorAndWeights {get; protected set; }
    public AgentStateComposite(Agent agent, Dictionary<ISubState, float> behaviorAndWeights){
        Agent = agent;
        BehaviorAndWeights = behaviorAndWeights;
    }

    public SteeringOutput GetSteering(){
        Vector3 pre_r_linear = Vector3.zero;
        float pre_r_angular = 0f;
        int sum_linear = 0;
        int sum_angular = 0;
        foreach(var b in BehaviorAndWeights){
            SteeringOutput steering = b.Key.GetSteering();
            if(steering.linearAcceleration != null){
                pre_r_linear += steering.linearAcceleration * b.Value ?? Vector3.zero;
                sum_linear++;
            }
            if(steering.angularAcceleration != null){
                pre_r_angular += steering.angularAcceleration * b.Value  ?? 0f;
                sum_angular++;
            }
        }
        Vector3? r_linear = null; //todo - remove nullables?
        float? r_angular = null;

        if(sum_linear != 0){
            r_linear = Vector3.ClampMagnitude(pre_r_linear/sum_linear, Agent.MaxAcceleration);
        } 
        if(sum_angular != 0){
            r_angular = Mathf.Clamp(pre_r_angular/sum_angular, -Agent.MaxAngularAcceleration_Y, Agent.MaxAngularAcceleration_Y);
        }

        return new SteeringOutput(r_linear, r_angular);
    }

    public void OnDrawGizmo(){
        foreach(var b in BehaviorAndWeights){
            b.Key.OnDrawGizmo();
        }
    }

    public void OnStateEnter() {
        foreach(var b in BehaviorAndWeights){
            b.Key.OnStateEnter();
        }
    }

    public void OnStateExit() {
        foreach(var b in BehaviorAndWeights){
            b.Key.OnStateExit();
        }
    }
}

public interface ISubState {
    // Agent Agent {get;  }
    abstract void OnStateEnter();
    abstract void OnStateExit();
    abstract void OnDrawGizmo();
    abstract SteeringOutput GetSteering();
}

public abstract class PositionalSubState<TPos> : ISubState where TPos : IPositionSteer{
    public Agent Agent {get; protected set; }
    abstract public TPos PosSteer {get; protected set;}
    public SteeringOutput GetSteering() => new SteeringOutput(PosSteer.GetPositionSteering(Agent), null);
    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }
    public virtual void OnDrawGizmo() { }
}


public abstract class SeparableSubState<TPos, TRot> : ISubState where TPos : IPositionSteer where TRot : IRotationSteer  {
    public Agent Agent {get; protected set; }
    abstract public TPos PosSteer {get; protected set;}
    abstract public TRot RotSteer {get; protected set;}
    public SteeringOutput GetSteering() => new SteeringOutput(PosSteer.GetPositionSteering(Agent), RotSteer.GetRotationSteering(Agent));
    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }
    public virtual void OnDrawGizmo() { }

}

public abstract class JoinedSubState<T> : ISubState where T : IPositionSteer, IRotationSteer {
    public Agent Agent {get; protected set; }
    abstract public T Steer {get; protected set; }
    public SteeringOutput GetSteering() => new SteeringOutput(Steer.GetPositionSteering(Agent), Steer.GetRotationSteering(Agent));
    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }
    public virtual void OnDrawGizmo() { }

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

public class SeparationState<T> : PositionalSubState<T> where T : SeparationSteer, new() {
    override public T PosSteer {get; protected set; }
    public SeparationState(Agent agent){
        Agent = agent;
        PosSteer = new T();
    }

    public override void OnDrawGizmo() { 
        Gizmos.DrawWireSphere(PosSteer.ClosestObstaclePos, 4f);

        switch(PosSteer){
            case ConeCheckSteer c: //todo separate states?
                float angle = Mathf.Acos(Agent.ConeThreshold);
                Vector3 angleA = DirFromAngle(angle/2 + (Agent.transform.eulerAngles.y - 90f) * Mathf.Deg2Rad);
                Vector3 angleB = DirFromAngle(-angle/2 + (Agent.transform.eulerAngles.y - 90f) * Mathf.Deg2Rad);
                Gizmos.DrawLine(Agent.transform.position, Agent.transform.position + angleA * Agent.Threshold);
                Gizmos.DrawLine(Agent.transform.position, Agent.transform.position + angleB * Agent.Threshold);
                break;
            case RayCastSteer r:
                Gizmos.DrawLine(Agent.transform.position, Agent.transform.position + (-Agent.transform.right * Agent.Threshold));
                break;
            default:
                break;
        }
       


    }

    private Vector3 DirFromAngle(float angleInRad) => new Vector3(Mathf.Sin(angleInRad), 0, Mathf.Cos(angleInRad));

}





