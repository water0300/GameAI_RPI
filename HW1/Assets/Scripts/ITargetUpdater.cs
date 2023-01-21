using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderTargetUpdater : ITargetPositionUpdater, ITargetRotationUpdater
{
    public float WanderOrientation {get; private set; } = 0f;

    public Vector3 GetTargetPosition(Agent agent)
    {
       return (agent.transform.position + agent.wanderOffset * agent.transform.rotation.AsNormVector()) + agent.wanderRadius * agent.Target.rotation.AsNormVector();

    }

    public Quaternion GetTargetRotation(Agent agent)
    {
        WanderOrientation += Utilities.RandomBinomial() * agent.wanderRate; //todo smoothdamp?
        // WanderOrientation = Mathf.SmoothDampAngle(WanderOrientation, Utilities.RandomBinomial() * agent.wanderRate, ref v, 0.1f); //todo smoothdamp?
        return Quaternion.AngleAxis(WanderOrientation + agent.transform.rotation.eulerAngles.y, Vector3.up);
    }
}