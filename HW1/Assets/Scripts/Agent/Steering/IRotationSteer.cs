using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRotationSteer {
    float? GetRotationSteering(Agent agent);
}

public interface IRotationSteer<T> : IRotationSteer where T : ITargetRotationUpdater, new(){
    T TargetRotationUpdater {get; }
}

public class AlignSteer<T> : IRotationSteer<T> where T : ITargetRotationUpdater, new() {
    public T TargetRotationUpdater {get; protected set; }
    public AlignSteer(){
        TargetRotationUpdater = new T();
    }
    public virtual float? GetRotationSteering(Agent agent) {
        TargetRotationUpdater.UpdateTargetRotation(agent);
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