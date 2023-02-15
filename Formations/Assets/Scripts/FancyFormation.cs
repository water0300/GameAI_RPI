using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class FancyFormationPattern : IFormationPattern {

    public PositionOrientation GetDriftOffset(List<FormationManager.SlotAssignment> assignments, FormationManager formationManager) {
        // return new PositionOrientation();
        PositionOrientation result = new PositionOrientation();
        foreach(var assignment in assignments){
            PositionOrientation location = GetSlotLocation(assignment.slotNumber, formationManager);
            result.position += location.position;
            result.orientationDeg += location.orientationDeg;
        }
        result.position /= assignments.Count;
        result.orientationDeg /= assignments.Count;
        return result;
    }

    public PositionOrientation GetSlotLocation(int slotNumber, FormationManager formationManager) {
        

        float leaderRotation = formationManager.leader.transform.eulerAngles.z;
        float angle = formationManager.spreadAngle;        
        float lerplimit = formationManager.lerpAngleLimit;

        //temp
        float radius;

        //angle : 70 to 90 
        //70- 70 = 0
        //90 - 70 = 20
        //(angle - lerplimit) / abs(angle-lerplimit)
        if(angle > lerplimit){
            radius = formationManager.characterRadius * Mathf.Lerp((slotNumber/2 + 1), (slotNumber + 1), (angle - lerplimit) / Mathf.Abs(90-lerplimit)); //distance
            // Debug.Log("yewot");
            // Debug.Log((angle - lerplimit) / Mathf.Abs(90-lerplimit));
        } else {
            radius = formationManager.characterRadius * (slotNumber/2 + 1); //distance
        }

        //given: leader rotation
        Vector2 newPosition;
        if(slotNumber % 2 == 0){
            newPosition = new Vector2(
                radius * Mathf.Cos((-angle + leaderRotation) * Mathf.Deg2Rad),
                radius * Mathf.Sin((-angle + leaderRotation) * Mathf.Deg2Rad)
            ) ;
        } else {
            newPosition = new Vector2(
                radius * -Mathf.Cos((-angle - leaderRotation) * Mathf.Deg2Rad),
                radius * Mathf.Sin((-angle - leaderRotation) * Mathf.Deg2Rad)
            ) ;
        }
            

        // Vector2 newPosition = new Vector2(
        //     radius * lOrR * Mathf.Cos(angle* Mathf.Deg2Rad) * Mathf.Cos(formationManager.tempAngle * Mathf.Deg2Rad),
        //     radius * Mathf.Sin(angle * Mathf.Deg2Rad) * (Mathf.Sin(formationManager.tempAngle * Mathf.Deg2Rad))
        // ) ;


        // <|
        // Debug.Log(newPosition);
        return new PositionOrientation(newPosition, 0f); //formationManager.leader.transform.eulerAngles.z
    }

    public bool SupportsSlots(int slotCount) => true;
}