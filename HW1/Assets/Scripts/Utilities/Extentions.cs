using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 static public class Extentions
 {
    static public Vector3 XZPlane(this Vector3 vec) => new Vector3(vec.x,0,vec.z);
    static public Vector3 XZPlane(this Vector2 vec) => new Vector3(vec.x,0,vec.y);
    static public Vector3 AsNormVector(this Quaternion q) => (q * Vector3.left).normalized; //note Vector3.left is relative to the direction of the agent itself
    static public Vector3 AsVector(this Quaternion q) => (q * Vector3.left); //note Vector3.left is relative to the direction of the agent itself
    static public Vector3 ExcludeY(this Vector3 vec) => new Vector2(vec.x, vec.z);
    

 }

