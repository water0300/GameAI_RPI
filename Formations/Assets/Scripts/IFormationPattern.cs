using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IFormationPattern {
    FormationManager.PositionOrientation GetDriftOffset(List<FormationManager.SlotAssignment> slotAssignments);
    FormationManager.PositionOrientation GetSlotLocation(int slotNumber);
    bool SupportsSlots(int slotCount);
}
