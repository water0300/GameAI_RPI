using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ISteer : IPositionSteer, IRotationSteer {}
public class WanderSteer : AlignSteer, ISteer {
    public ITargetPositionUpdater TargetPositionUpdater { get; private set; }

    public WanderSteer() {
        WanderTargetUpdater wtu = new WanderTargetUpdater();
        TargetRotationUpdater = wtu;
        TargetPositionUpdater = wtu;
    }

    public override float? GetRotationSteering(Agent agent){
        agent.Target.rotation = TargetRotationUpdater.GetTargetRotation(agent);
        agent.Target.position = TargetPositionUpdater.GetTargetPosition(agent);
        return GetAlignSteering(agent);
    }

    public Vector3? GetPositionSteering(Agent agent){ 
        agent.statusText = "Wandering";
        return agent.MaxAcceleration * agent.transform.rotation.AsNormVector();
    }
    

}
