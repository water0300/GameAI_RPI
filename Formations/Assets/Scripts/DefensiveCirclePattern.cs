using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class DefensiveCirclePattern : IFormationPattern {
    // public float characterRadius; 
    public int CalculateNumSlots(List<FormationManager.SlotAssignment> assignments) {
        var x = 1 + assignments.Aggregate((max, next) => next.slotNumber > max.slotNumber ? next : max).slotNumber;
    
        // Debug.Log(x);
        return x;
    }
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
        float angleAroundCircleRad = (float) slotNumber /  (float) formationManager.numberOfSlots * Mathf.PI * 2;
        angleAroundCircleRad += formationManager.leader.transform.eulerAngles.z * Mathf.Deg2Rad;
        float radius = (float) formationManager.characterRadius /  (float) Mathf.Sin(Mathf.PI / formationManager.numberOfSlots);
        // Debug.Log($"{radius}, {characterRadius}, {numberOfSlots}");

        Vector2 newPosition = new Vector2(
            radius * Mathf.Cos(angleAroundCircleRad),
            radius * Mathf.Sin(angleAroundCircleRad)
        );
        // Debug.Log(newPosition);
        return new PositionOrientation(newPosition, angleAroundCircleRad * Mathf.Rad2Deg);
    }

    public bool SupportsSlots(int slotCount) => true;
}