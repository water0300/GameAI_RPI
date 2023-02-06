using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IFormationPattern {
    int CalculateNumSlots(List<FormationManager.SlotAssignment> assignments);
    PositionOrientation GetDriftOffset(List<FormationManager.SlotAssignment> assignments, float characterRadius);
    PositionOrientation GetSlotLocation(int slotNumber, int numberOfSlots, float characterRadius);
    bool SupportsSlots(int slotCount);
}
