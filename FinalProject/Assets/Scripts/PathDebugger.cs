using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(NavMeshAgent))]
public class PathDebugger : MonoBehaviour
{
    NavMeshAgent agent;
    LineRenderer lr;
    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        lr = GetComponent<LineRenderer>();
    }

    private void Update() {
        if(agent.hasPath){
            lr.positionCount = agent.path.corners.Length;
            lr.SetPositions(agent.path.corners);
            lr.enabled = true;
        } else {
            lr.enabled = false;
        }
    }
}
