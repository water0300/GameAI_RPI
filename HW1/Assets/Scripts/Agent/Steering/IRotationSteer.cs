using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRotationSteer{
    ITargetRotationUpdater TargetRotationUpdater {get; }
    float? GetRotationSteering(Agent agent);
}

public class AlignSteer : IRotationSteer {
    public ITargetRotationUpdater TargetRotationUpdater {get; protected set; }
    protected AlignSteer() {}
    public AlignSteer(ITargetRotationUpdater targetRotationUpdater){
        TargetRotationUpdater = targetRotationUpdater;
    }
    public virtual float? GetRotationSteering(Agent agent) {
        agent.Target.rotation = TargetRotationUpdater.GetTargetRotation(agent);
        return GetAlignSteering(agent);
    }

    protected float? GetAlignSteering(Agent agent){
        float rotation = agent.transform.rotation.eulerAngles.y - agent.Target.transform.rotation.eulerAngles.y;
        rotation %= 360;
        rotation = rotation > 180 ? rotation - 360 : (rotation < -180 ? rotation + 360 : rotation);
        float rotationSize = Mathf.Abs(rotation);
        if(rotationSize < agent.TargetAlignWindow){
            return null;
        }
        // Debug.Log($"Rotation: {rotation}");

        float targetAngularSpeed_Y;
        if(rotationSize > agent.SlowAlignWindow){
            targetAngularSpeed_Y = agent.MaxAngularSpeed_Y; 
        } else {
            targetAngularSpeed_Y = agent.MaxAngularSpeed_Y * rotationSize / agent.SlowAlignWindow;
        }
        // Debug.Log($"TargetRotation: {targetRotation}");

        targetAngularSpeed_Y *= rotation/rotationSize;
        return Mathf.Clamp((targetAngularSpeed_Y - agent.AngularSpeed_Y) / agent.TimeToAlign, -agent.MaxAngularAcceleration_Y, agent.MaxAngularAcceleration_Y);
    }
}