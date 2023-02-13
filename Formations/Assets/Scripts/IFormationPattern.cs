using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IFormationPattern {
    // int CalculateNumSlots(List<FormationManager.SlotAssignment> assignments);
    PositionOrientation GetDriftOffset(List<FormationManager.SlotAssignment> assignments, FormationManager formationManager);
    PositionOrientation GetSlotLocation(int slotNumber, FormationManager formationManager);
    bool SupportsSlots(int slotCount);
}
