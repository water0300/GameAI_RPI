using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

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

    //index 0: move vector x (+1 = right, -1 = left)
    //index 1: move vector y (+1 = up, -1 = down)
    //index 2: move vector z (+1 = forward, -1 = backward)
    //index 3: pitck angle (+1 = pitch up, -1 = pitch down)
    //index 4: yaw angle (+1` = turn right, -1 = turn left)
    public override void OnActionReceived(ActionBuffers actions)
    {
        if(frozen){
            return;
        }

        Vector3 move = new Vector3(actions.ContinuousActions[0], actions.ContinuousActions[1], actions.ContinuousActions[2]);
        rigidbody.AddForce(move * moveForce);

        Vector3 rotationVector = transform.rotation.eulerAngles;
        float pitchChange = actions.ContinuousActions[3];
        float yawChange = actions.ContinuousActions[4];

        smoothPitchChange = Mathf.MoveTowards(smoothPitchChange, pitchChange, 2f * Time.fixedDeltaTime);
        smoothYawChange = Mathf.MoveTowards(smoothYawChange, yawChange, 2f * Time.fixedDeltaTime);

        float pitch = rotationVector.x + smoothPitchChange * Time.fixedDeltaTime * pitchSpeed;
        if(pitch > 180f){
            pitch -= 360f;
        }
        pitch = Mathf.Clamp(pitch, -MaxPitchAngle, MaxPitchAngle);
        float yaw = rotationVector.y + smoothYawChange * Time.fixedDeltaTime * yawSpeed;

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if(nearestFlower == null){
            sensor.AddObservation(new float[10]);
            return;
        }

        //4 observations (quaternion)
        sensor.AddObservation(transform.localRotation.normalized);
    
        //3 observations
        Vector3 toFlower = nearestFlower.FlowerCenterPosition - beakTip.position;
        sensor.AddObservation(toFlower.normalized);

        //+1 in front of flower, -1 is behind
        sensor.AddObservation(Vector3.Dot(toFlower.normalized, -nearestFlower.FlowerUpVector.normalized));

        //pointing: +1 at flower, -1 away
        sensor.AddObservation(Vector3.Dot(beakTip.forward.normalized, -nearestFlower.FlowerUpVector.normalized));

        //relative distance of beak tip to flower
        sensor.AddObservation(toFlower.magnitude / FlowerArea.AreaDiameter);

        //10 total observations
    }

    public override void Heuristic(in ActionBuffers actionsOut){
        Vector3 forward = Vector3.zero;
        Vector3 left = Vector3.zero;
        Vector3 up = Vector3.zero;
        float pitch = 0f;
        float yaw = 0f;

        if(Input.GetKey(KeyCode.W)){
            forward = transform.forward;
        } else if(Input.GetKey(KeyCode.S)){
            forward = -transform.forward;
        }

        if(Input.GetKey(KeyCode.A)){
            left = -transform.right;
        } else if(Input.GetKey(KeyCode.D)){
            left = transform.right;
        }

        if(Input.GetKey(KeyCode.E)){
            up = transform.up;
        } else if(Input.GetKey(KeyCode.C)){
            up = -transform.up;
        }

        if(Input.GetKey(KeyCode.UpArrow)){
            pitch = 1f;
        } else if(Input.GetKey(KeyCode.DownArrow)){
            pitch = -1f;
        }

        if(Input.GetKey(KeyCode.RightArrow)){
            yaw = 1f;
        } else if(Input.GetKey(KeyCode.LeftArrow)){
            yaw = -1f;
        }

        Vector3 combined = (forward + left + up).normalized;
        var cao = actionsOut.ContinuousActions;
        cao[0] = combined.x;
        cao[1] = combined.y;
        cao[2] = combined.z;
        cao[3] = pitch;
        cao[4] = yaw;
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
        }
        Debug.Assert(safePositionFound, "Could not find safe pos to spawn");

        transform.position = potentialPosition;
        transform.rotation = potentialRotation;
    }

    private void UpdateNearestFlower(){
        foreach(Flower flower in flowerArea.Flowers){
            if(nearestFlower == null && flower.HasNectar){
                nearestFlower = flower;
            } else if(flower.HasNectar){
                float distanceToFlower = Vector3.Distance(flower.transform.position, beakTip.position);
                float distanceToCurrentNearestFlower = Vector3.Distance(nearestFlower.transform.position, beakTip.position);

                if(!nearestFlower.HasNectar || distanceToFlower < distanceToCurrentNearestFlower){
                    nearestFlower = flower;
                }
            }
        }
    }

    public void FreezeAgent(){
        Debug.Assert(trainingMode == false, "Freeze/Unfreeze not supported in training");
        frozen = true;
        rigidbody.Sleep();
    }
    public void Unfreeze(){
        Debug.Assert(trainingMode == false, "Freeze/Unfreeze not supported in training");
        frozen = false;
        rigidbody.WakeUp();
    }

    private void OnTriggerEnter(Collider other) {
        TriggerEnterOrStay(other);
    }

    private void OnTriggerStay(Collider other) {
        TriggerEnterOrStay(other);
    }

    private void TriggerEnterOrStay(Collider collider){
        if(collider.CompareTag("nectar")){
            Vector3 closestPointToBeakTip = collider.ClosestPoint(beakTip.position);

            if(Vector3.Distance(beakTip.position, closestPointToBeakTip) < BeakTipRadius){
                Flower flower = flowerArea.GetFlowerFromNectar(collider);

                float nectarReceived = flower.Feed(0.01f);
                NectarObtained += nectarReceived;

                if(trainingMode){
                    float bonus = .02f * Mathf.Clamp01(Vector3.Dot(transform.forward.normalized, -nearestFlower.FlowerUpVector.normalized));
                    AddReward(0.03f + bonus);
                }

                if(!flower.HasNectar){
                    UpdateNearestFlower();
                }
            }

        }
    }

    private void OnCollisionEnter(Collision other) {
        if(trainingMode && other.collider.CompareTag("boundary")){
            AddReward(-.3f);
        }
    }

    private void Update() {
        if(nearestFlower != null){
            Debug.DrawLine(beakTip.position, nearestFlower.FlowerCenterPosition, Color.green);
        }
    }

    private void FixedUpdate() {
        if(nearestFlower != null && !nearestFlower.HasNectar){
            UpdateNearestFlower();
        }
    }

}
