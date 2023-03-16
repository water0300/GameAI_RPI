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
}
