using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetRotationUpdater {
    void UpdateTargetRotation(Agent agent);
}

public class AlignTargetRotationUpdater : ITargetRotationUpdater {
    public void UpdateTargetRotation(Agent agent) => agent.Target.rotation = agent.TargetRB.rotation;

}

public class FaceTargetRotationUpdater : ITargetRotationUpdater {
    public void UpdateTargetRotation(Agent agent){
        Vector3 direction = agent.TargetRB.position - agent.transform.position;
        if(direction.sqrMagnitude == 0){
            agent.Target.rotation = agent.TargetRB.rotation;
        } else {
            // Debug.Log(Mathf.Atan2(-direction.x, -direction.z) * Mathf.Rad2Deg);
            agent.Target.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.z, -direction.x) * Mathf.Rad2Deg, Vector3.up); //trial and error
        }
    }

}

public class HideFromTargetRotationUpdater : ITargetRotationUpdater {
    public void UpdateTargetRotation(Agent agent){
        Vector3 direction =  agent.transform.position - agent.TargetRB.position;
        if(direction.sqrMagnitude == 0){
            agent.Target.rotation = agent.TargetRB.rotation;
        } else {
            // Debug.Log(Mathf.Atan2(-direction.x, -direction.z) * Mathf.Rad2Deg);
            agent.Target.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.z, -direction.x) * Mathf.Rad2Deg, Vector3.up); //trial and error
        }
    }

}


public class LookWhereYoureGoingTargetRotationUpdater : ITargetRotationUpdater {
    public void UpdateTargetRotation(Agent agent){
        if(agent.Velocity.sqrMagnitude == 0){
            agent.Target.rotation = agent.transform.rotation;
        } else {
            // Debug.Log(Mathf.Atan2(-direction.x, -direction.z) * Mathf.Rad2Deg);
            agent.Target.rotation = Quaternion.AngleAxis(Mathf.Atan2(agent.Velocity.z, -agent.Velocity.x) * Mathf.Rad2Deg, Vector3.up); //trial and error
        }
    }

}