using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class HummingbirdAgent : Agent {
    [Tooltip("Force to apply when moving")]
    public float moveForce = 2f;
    [Tooltip("pitch up/down speed")]
    public float pitchSpeed = 100f;
    [Tooltip("speed to rotate around the up axis")]
    public float yawSpeed = 100f;
    [Tooltip("Transform at beak tip")]
    public Transform beakTip;
    [Tooltip("agent camera")]
    public Camera agentCamera;
    [Tooltip("training vs gameplay mode")]
    public bool trainingMode;

    new private Rigidbody rigidbody;
    private FlowerArea flowerArea;
    private Flower nearestFlower;
    private float smoothPitchChange = 0f;
    private float smoothYawChange = 0f;
    private const float MaxPitchAngle = 80f;
    private const float BeakTipRadius = 0.008f;
    private bool frozen = false;
    public float NectarObtained {get; private set;}

    public override void Initialize()
    {
        rigidbody = GetComponent<Rigidbody>();
        flowerArea = GetComponentInParent<FlowerArea>();
        if(!trainingMode) MaxStep = 0;
    }

    public override void OnEpisodeBegin()
    {
        if(trainingMode){
            flowerArea.ResetFlowers();
        }
        NectarObtained = 0f;

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        bool inFrontOfFlower = true;
        if(trainingMode){
            inFrontOfFlower = Random.value > .5f;
        }

        MoveToSafeRandomPosition(inFrontOfFlower);
        UpdateNearestFlower();
    }

    private void MoveToSafeRandomPosition(bool inFrontOfFlower){
        bool safePositionFound = false;
        int attemptsRemaining = 100;
        Vector3 potentialPosition = Vector3.zero;
        Quaternion potentialRotation = new Quaternion();

        while(!safePositionFound && attemptsRemaining > 0){
            attemptsRemaining--;
            if(inFrontOfFlower){
                Flower randomFlower = flowerArea.Flowers[Random.Range(0, flowerArea.Flowers.Count)];

                float distanceFromFlower = Random.Range(0.1f, 0.2f);
                potentialPosition = randomFlower.transform.position + randomFlower.FlowerUpVector * distanceFromFlower;

                Vector3 toFlower = randomFlower.FlowerCenterPosition - potentialPosition;
                potentialRotation = Quaternion.LookRotation(toFlower, Vector3.up);
            } else {
                float height = Random.Range(1.2f, 2.5f);
                float radius = Random.Range(2f, 7f);

                Quaternion direction = Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f);
                potentialPosition = flowerArea.transform.position + Vector3.up * height + direction * Vector3.forward * radius;

                float pitch = Random.Range(-60f, 60f);
                float yaw = Random.Range(-180f, 180f);
                potentialRotation = Quaternion.Euler(pitch, yaw, 0f);
            }

            var colliders = Physics.OverlapSphere(potentialPosition, 0.05f);
            safePositionFound = colliders.Length == 0;

            Debug.Assert(safePositionFound, "Could not find safe pos to spawn");

            transform.position = potentialPosition;
            transform.rotation = potentialRotation;
        }
    }

    private void UpdateNearestFlower(){
        
    }
}
