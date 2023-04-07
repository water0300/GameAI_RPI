using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {
    
    public static bool CompareFloats(float a, float b, float tolerance = 0.0001f){
        return Mathf.Abs(a - b) <= tolerance;
    }
}
