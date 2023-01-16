using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 static public class VectorExtensions
 {
    static public Vector3 XZPlane(this Vector3 vec)
    {
        return new Vector3(vec.x,0,vec.z);
    }
 }