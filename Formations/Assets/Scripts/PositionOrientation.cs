using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionOrientation{
    public Vector2 position;
    public float orientationDeg;
    public Quaternion Rotation {get => Quaternion.Euler(0f, 0f, orientationDeg); }

    public PositionOrientation(){
        this.position = Vector2.zero;
        this.orientationDeg = 0f;
    }
    public PositionOrientation(Vector2 position, float orientation){
        this.position = position;
        this.orientationDeg = orientation;
    }
    public PositionOrientation(Vector2 position, Quaternion rotation){
        this.position = position;
        this.orientationDeg = rotation.eulerAngles.z;
    }
    
}