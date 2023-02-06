using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float speed = 3f;
    public float rotSpeed = 2f;
    public Vector2 InputAxis {get; set;}
    public float RotationAxis {get; set;}

    // Update is called once per frame
    void Update(){
        transform.position += InputAxis.XYPlane() * speed * Time.deltaTime;
        transform.eulerAngles += Vector3.forward * RotationAxis * rotSpeed * Time.deltaTime;
    }
}
