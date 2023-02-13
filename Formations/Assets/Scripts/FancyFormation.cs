using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class FancyFormationPattern : IFormationPattern {

    public PositionOrientation GetDriftOffset(List<FormationManager.SlotAssignment> assignments, FormationManager formationManager) {
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
        

        float angle = formationManager.spreadAngle * -1;        

        float radius = formationManager.characterRadius * (slotNumber/2 + 1); //distance
        float lOrR = slotNumber == 0? 1 : ((float)slotNumber/2)/(slotNumber/2) == 1 ? 1 : -1; //left or right
        
        Debug.Log(radius);
        // Debug.Log($"{radius}, {characterRadius}, {numberOfSlots}");

        Vector2 newPosition = new Vector2(
            radius * lOrR * Mathf.Cos(angle* Mathf.Deg2Rad),
            radius * Mathf.Sin(angle * Mathf.Deg2Rad)
        ) ;
        // Debug.Log(newPosition);
        return new PositionOrientation(newPosition, formationManager.transform.eulerAngles.z);
    }

    public bool SupportsSlots(int slotCount) => true;
}