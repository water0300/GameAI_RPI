using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetPositionUpdater { //interface exists because seek and arrive reuse the same code
    Vector3 GetTargetPosition(Agent agent);
}

public class DirectTargetPositionUpdater : ITargetPositionUpdater {
    public Vector3 GetTargetPosition(Agent agent) => agent.TargetRB.position;

}

public class LookaheadTargetPositionUpdater : ITargetPositionUpdater {
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

}
