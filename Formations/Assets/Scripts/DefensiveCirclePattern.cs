using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class DefensiveCirclePattern : IFormationPattern {
    // public float characterRadius; 
    public int CalculateNumSlots(List<FormationManager.SlotAssignment> assignments) {
        var x = 1 + assignments.Aggregate((max, next) => next.slotNumber > max.slotNumber ? next : max).slotNumber;
    
        Debug.Log(x);
        return x;
    }
    public PositionOrientation GetDriftOffset(List<FormationManager.SlotAssignment> assignments, float characterRadius) {
        PositionOrientation result = new PositionOrientation();
        foreach(var assignment in assignments){
            PositionOrientation location = GetSlotLocation(assignment.slotNumber, characterRadius);
            result.position += location.position;
            result.orientationDeg += location.orientationDeg;
        }
        result.position /= assignments.Count;
        result.orientationDeg /= assignments.Count;
        return result;
    }

    public PositionOrientation GetSlotLocation(int slotNumber, float characterRadius) {
        float angleAroundCircleRad = (float) slotNumber /  (float) FormationManager.numberOfSlots * Mathf.PI * 2;
        float radius = (float) characterRadius /  (float) Mathf.Sin(Mathf.PI / FormationManager.numberOfSlots);
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