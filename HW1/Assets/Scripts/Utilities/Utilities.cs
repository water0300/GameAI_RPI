using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {
    public static float RandomBinomial() => Random.value - Random.value;
    
    public static Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 end, Vector3 point)
    {
        //Get heading
        Vector3 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        return origin + heading * Mathf.Clamp(Vector3.Dot(point - origin, heading), 0f, magnitudeMax);
    
        //tldr the Mathf  Clamp term finds a "percentage" along the origin-end line that is perpendicular via dot
        //clamped since the point could be like -- x (out of the "dot" range thing)
    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value){
         Vector3 AB = b - a;
         Vector3 AV = value - a;
         return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
     }
}
