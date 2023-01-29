using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderTargetUpdater : ITargetPositionUpdater, ITargetRotationUpdater
{
    public float WanderOrientation {get; set; } = 0f;

    public void UpdateTargetPosition(Agent agent)
    {
       agent.Target.position = (agent.transform.position + agent.WanderOffset * agent.transform.rotation.AsNormVector()) + agent.WanderRadius * agent.Target.rotation.AsNormVector();

    }
    // private float _v;
    public void UpdateTargetRotation(Agent agent)
    {
        // WanderOrientation += Mathf.SmoothDampAngle(WanderOrientation, Utilities.RandomBinomial() * agent.WanderRate, ref _v, 0.2f);
        WanderOrientation += Utilities.RandomUniform() * agent.WanderRate; //todo smoothdamp?
        // WanderOrientation = Mathf.SmoothDampAngle(WanderOrientation, Utilities.RandomBinomial() * agent.wanderRate, ref v, 0.1f); //todo smoothdamp?
        agent.Target.rotation = Quaternion.AngleAxis(WanderOrientation + agent.transform.rotation.eulerAngles.y, Vector3.up);
    }
}