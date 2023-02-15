using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System; 


public class FormationManager : MonoBehaviour { //should be singleton
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
    // public static int SlotNumber {get => characterCount; } //todo temp

    [Header("Prefab Refs")]
    public Character characterPrefab;

    [Header("In Scene Refs")]
    public Transform leader;

    [Header("Properties")]
    public float characterRadius;
    public int numberOfSlots = 12; 
    [Range(0f, 90f)] public float defaultSpreadAngle = 90f;
    public float rSpreadAngle = 90f;
    public float lSpreadAngle = 90f;
    [Range(0f, 90f)] public float lerpAngleLimit = 70f;
    public float tickrateSeconds = 0.4f;
    private void Start() {
        Pattern = new FancyFormationPattern();
        for(int i = 0; i < numberOfSlots; i++){
            Character c = Instantiate(characterPrefab, transform.position, transform.rotation);
            c.name = $"clone_{i}";
            AddCharacter(c);
        }
        UpdateSlots();

    }

    private void Update() {
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
        DriftOffset = Pattern.GetDriftOffset(SlotAssignments, this);
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
            PositionOrientation slot = Pattern.GetSlotLocation(slotNumber, this);
            // Debug.Log($"{anchor.position} + {slot.Rotation.eulerAngles} * {slot.position}");
            
            // Vector3 dir = Mathf.atan
            // Debug.Log(slot.Rotation * slot.position);

            SlotAssignments[i].character.SetTarget( 
                new PositionOrientation(
                    anchor.position + slot.position - DriftOffset.position,
                    anchor.orientationDeg + slot.orientationDeg - DriftOffset.orientationDeg
                )
            );
                
        }
    }

    public PositionOrientation GetAnchorPoint(){
        return new PositionOrientation(leader.position, leader.rotation);
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(DriftOffset.position, 0.5f);
    }
}
