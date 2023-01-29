using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderSteer : AlignSteer<WanderTargetUpdater>, IPositionSteer<WanderTargetUpdater> {
    public WanderTargetUpdater TargetPositionUpdater { get; private set; }

    public WanderSteer() {
        WanderTargetUpdater wtu = new WanderTargetUpdater();
        TargetRotationUpdater = wtu;
        TargetPositionUpdater = wtu;
    }

    public override float? GetRotationSteering(Agent agent){
        TargetRotationUpdater.UpdateTargetRotation(agent);
        TargetPositionUpdater.UpdateTargetPosition(agent);
        return GetAlignSteering(agent);
    }

    public Vector3? GetPositionSteering(Agent agent){ 
        agent.statusText = "Wandering";
        return agent.MaxAcceleration * agent.transform.rotation.AsNormVector();
    }
    

}
