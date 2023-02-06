using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    public PositionOrientation Target {get; private set; }
    public void SetTarget(PositionOrientation positionOrientation){
        Target = positionOrientation;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(Target.position, 3f);
    }
}
