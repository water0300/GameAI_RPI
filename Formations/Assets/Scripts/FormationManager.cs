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
    public List<SlotAssignment> SlotAssignments {get; private set; } = new List<SlotAssignment>();
    public PositionOrientation DriftOffset {get; private set; }
    public IFormationPattern Pattern {get; private set; }

    [Header("Prefab Refs")]
    public Character characterPrefab;

    [Header("In Scene Refs")]
    public Transform leader;

    [Header("Properties")]
    public float characterRadius;
    public int characterCount;
    public float tickrateSeconds = 0.4f;
    private void Start() {
        Pattern = new DefensiveCirclePattern();
        for(int i = 0; i < characterCount; i++){
            Character c = Instantiate(characterPrefab);
            AddCharacter(c);
        }
        UpdateSlots();

    }

    private IEnumerator FormationUpdater(){
        while(true){
            UpdateSlots();
            yield return new WaitForSeconds(tickrateSeconds);
        }
    }


    public void UpdateSlotAssignments(){
        for(int i = 0; i < SlotAssignments.Count; i++){
            SlotAssignments[i].slotNumber = i;
        }
        DriftOffset = Pattern.GetDriftOffset(SlotAssignments, characterRadius);
    }

    public bool AddCharacter(Character character){
        if(!Pattern.SupportsSlots(SlotAssignments.Count + 1)){
            return false;
        }

        SlotAssignments.Add(new SlotAssignment(character));
        UpdateSlotAssignments();
        return true;
    }

    public void RemoveCharacter(Character character){
        SlotAssignments.Remove(SlotAssignments.Where(item => item.character == character).First());
        UpdateSlotAssignments();
    }

    public void UpdateSlots(){
        PositionOrientation anchor = GetAnchorPoint();
        for(int i = 0; i < SlotAssignments.Count; i++){
            int slotNumber = SlotAssignments[i].slotNumber;
            PositionOrientation slot = Pattern.GetSlotLocation(slotNumber, pattern.CalculateNumSlots(SlotAssignments), characterRadius);
            // Debug.Log($"{anchor.position} + {slot.Rotation.eulerAngles} * {slot.position}");

            SlotAssignments[i].character.SetTarget(
                new PositionOrientation(
                    anchor.position + slot.Rotation * slot.position,
                    anchor.orientationDeg + slot.orientationDeg - DriftOffset.orientationDeg
                )
            );
        }
    }

    public PositionOrientation GetAnchorPoint(){
        return new PositionOrientation(leader.position, leader.rotation);
    }
    
}
