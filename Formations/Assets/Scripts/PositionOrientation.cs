using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionOrientation{
    public Vector3 position;
    public float orientationDeg;
    public Quaternion Rotation {get => Quaternion.Euler(0f, 0f, orientationDeg); }

    public PositionOrientation(){
        this.position = Vector3.zero;
        this.orientationDeg = 0f;
    }
    public PositionOrientation(Vector3 position, float orientation){
        this.position = position;
        this.orientationDeg = orientation;
    }
    public PositionOrientation(Vector3 position, Quaternion rotation){
        this.position = position;
        this.orientationDeg = rotation.eulerAngles.z;
    }
    
}