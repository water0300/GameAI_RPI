using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System; 
public class FormationManager : MonoBehaviour {
    public class SlotAssignment {
        public Character character;
        public int slotNumber;
        public SlotAssignment(Character character){
            this.character = character;
        }
    }

    public struct PositionOrientation{
        public Vector3 position;
        public Quaternion rotation;
        public float Orientation {get => rotation.eulerAngles.y; }
        public PositionOrientation(Vector3 position, float orientation){
            this.position = position;
            this.rotation = Quaternion.Euler(0f, orientation, 0f);
        }
    }
    public List<SlotAssignment> slotAssignments = new List<SlotAssignment>();
    public PositionOrientation driftOffset;
    public IFormationPattern pattern;
    public void UpdateSlotAssignments(){
        for(int i = 0; i < slotAssignments.Count; i++){
            slotAssignments[i].slotNumber = i;
        }
        driftOffset = pattern.GetDriftOffset(slotAssignments);
    }

    public bool AddCharacter(Character character){
        if(!pattern.SupportsSlots(slotAssignments.Count + 1)){
            return false;
        }

        slotAssignments.Add(new SlotAssignment(character));
        UpdateSlotAssignments();
        return true;
    }

    public void RemoveCharacter(Character character){
        slotAssignments.Remove(slotAssignments.Where(item => item.character == character).First());
        UpdateSlotAssignments();
    }

    public void UpdateSlots(){
        PositionOrientation anchor = GetAnchorPoint();
        for(int i = 0; i < slotAssignments.Count; i++){
            int slotNumber = slotAssignments[i].slotNumber;
            PositionOrientation slot = pattern.GetSlotLocation(slotNumber);
            slotAssignments[i].character.SetTarget(
                new PositionOrientation(
                    anchor.position + slot.rotation * slot.position,
                    anchor.Orientation + slot.Orientation - driftOffset.Orientation
                )
            );
        }
    }

    public PositionOrientation GetAnchorPoint(){
        throw new NotImplementedException();
    }
    
}
