using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetPositionUpdater { //interface exists because seek and arrive reuse the same code
    void UpdateTargetPosition(Agent agent);
}

public class DirectTargetPositionUpdater : ITargetPositionUpdater {
    public void UpdateTargetPosition(Agent agent) => agent.Target.position = agent.TargetRB.position;

}

public class LookaheadTargetPositionUpdater : ITargetPositionUpdater {
    public void UpdateTargetPosition(Agent agent){
        Vector3 direction = agent.TargetRB.position - agent.transform.position;
        float distance = direction.magnitude;
        float speed = agent.Velocity.magnitude;
        float prediction;
        if (speed <= distance /agent.MaxPredictionLookahead){
            prediction = agent.MaxPredictionLookahead;
            // Debug.Log("max predict");
        } else {
            prediction = distance/speed;
            // Debug.Log("slowing down");

        }

        agent.Target.position = agent.TargetRB.position + agent.TargetRB.velocity * prediction;
    }

}

public class FollowPathTargetPositionUpdater : ITargetPositionUpdater {
    public float CurrentParam {get; set; } = 0f;
    public void UpdateTargetPosition(Agent agent){ //assume path is non null
        CurrentParam = agent.Path.GetParam(agent.transform.position, CurrentParam);
        agent.Target.position = agent.Path.GetTargetPosition(CurrentParam + agent.PathOffset);
    }

}

