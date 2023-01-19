using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetUpdater {
    Vector3 GetTargetPosition(Agent agent);
    Quaternion GetTargetRotation(Agent agent);
}

public class DirectTargetPositionUpdater : ITargetUpdater {
    public Vector3 GetTargetPosition(Agent agent) => agent.TargetRB.position;
    public Quaternion GetTargetRotation(Agent agent) => agent.TargetRB.rotation;


}

public class LookaheadTargetPositionUpdater : ITargetUpdater {
    public Vector3 GetTargetPosition(Agent agent){
        // Debug.Log("ayfasdf");
        Vector3 direction = agent.TargetRB.position - agent.transform.position;
        float distance = direction.magnitude;
        float speed = agent.Velocity.magnitude;
        float prediction;
        if (speed <= distance /agent.maxPredictionLookahead){
            prediction = agent.maxPredictionLookahead;
            Debug.Log("max predict");
        } else {
            prediction = distance/speed;
            Debug.Log("slowing down");

        }

        return agent.TargetRB.position + agent.TargetRB.velocity * prediction;
    }
    public Quaternion GetTargetRotation(Agent agent){
        Vector3 direction = agent.TargetRB.position - agent.transform.position;
        if(direction.sqrMagnitude == 0){
            return agent.TargetRB.rotation;
        } else {
            // Debug.Log(Mathf.Atan2(-direction.x, -direction.z) * Mathf.Rad2Deg);
            return Quaternion.AngleAxis(Mathf.Atan2(direction.z, -direction.x) * Mathf.Rad2Deg, Vector3.up); //trial and error
        }
    }

}
