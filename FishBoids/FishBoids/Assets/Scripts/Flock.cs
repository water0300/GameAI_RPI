using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {

    float speed;
    bool turning = false;

    void Start() {

        speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
    }


    void Update() {

        Bounds b = new Bounds(FlockManager.FM.transform.position, FlockManager.FM.swimLimits * 2.0f);

        if (!b.Contains(transform.position)) {

            turning = true;
        } else {

            turning = false;
        }

        if (turning) {

            Vector3 direction = FlockManager.FM.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                FlockManager.FM.rotationSpeed * Time.deltaTime);
        } else {


            if (Random.Range(0, 100) < 10) {

                speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
            }


            if (Random.Range(0, 100) < 10) {
                ApplyRules();
            }
        }

        this.transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
    }



    private void ApplyRules() {

        List<GameObject> gos = new List<GameObject>(FlockManager.FM.allFish);
        gos.Add(FlockManager.FM.obstacle1);
        gos.Add(FlockManager.FM.obstacle2);
        gos.Add(FlockManager.FM.obstacle3);

        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;

        float gSpeed = 0.01f;
        float mDistance;
        int groupSize = 0;

        bool avoidObstacle = false;

        foreach (GameObject go in gos) {

            if (go != this.gameObject) {

                mDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if (mDistance <= FlockManager.FM.neighbourDistance) {

                    vCentre += go.transform.position;
                    groupSize++;

                    if (mDistance < 1.5f) {

                        vAvoid = vAvoid + (this.transform.position - go.transform.position);
                    }
                    Flock anotherFlock;
                    if(go.TryGetComponent<Flock>(out anotherFlock)){
                        gSpeed = gSpeed + anotherFlock.speed;
                    }  else {
                        avoidObstacle = true;
                        // Debug.Log($"{go.name}, {this.name}, {vAvoid}");

                    }
                    
                }
            }
        }

        if (groupSize > 0) {

            vCentre = vCentre / groupSize + (FlockManager.FM.goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            if (speed > FlockManager.FM.maxSpeed) {

                speed = FlockManager.FM.maxSpeed;
            }
            Vector3 direction;
            if(avoidObstacle){
                direction = Vector3.Cross(vAvoid - transform.position, Vector3.up);
                transform.rotation = Quaternion.LookRotation(direction);
                return;
            }else {
                direction = (vCentre + vAvoid) - transform.position;

            }
            if (direction != Vector3.zero) {

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    FlockManager.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
}