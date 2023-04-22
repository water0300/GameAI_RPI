using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {
    
    public static LayerMask IgnoreLayer(int layerToIgnore){
        return ~(1 << layerToIgnore);
    }

    public static bool CompareFloats(float a, float b, float tolerance = 0.0001f){
        return Mathf.Abs(a - b) <= tolerance;
    }

    public static void DrawCircle(Vector3 center, float radius, Color color, float resolution = 40f){
        Gizmos.color = color;
        float deltaTheta = (2f*Mathf.PI) / resolution;
        float theta = 0f;
        Vector3 oldPos = center;
        for(int i = 0; i < resolution; i++){
            Vector3 pos = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
            Gizmos.DrawLine(oldPos, center + pos);
            oldPos = center + pos;
            theta += deltaTheta;
        }
    }

    public static void DrawWireArc(Vector3 position, Vector3 dir, float anglesRange, float radius, float maxSteps = 20)
    {
        var srcAngles = GetAnglesFromDir(position, dir);
        var initialPos = position;
        var posA = initialPos;
        var stepAngles = anglesRange / maxSteps;
        var angle = srcAngles - anglesRange / 2;
        for (var i = 0; i <= maxSteps; i++)
        {
            var rad = Mathf.Deg2Rad * angle;
            var posB = initialPos;
            posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

            Gizmos.DrawLine(posA, posB);

            angle += stepAngles;
            posA = posB;
        }
        Gizmos.DrawLine(posA, initialPos);
    }

    static float GetAnglesFromDir(Vector3 position, Vector3 dir)
    {
        var forwardLimitPos = position + dir;
        var srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);

        return srcAngles;
    }

    public static float RandomGaussian(float mean = 0f, float stdev = 1f){
        return mean + stdev * (Mathf.Sqrt(-2f * Mathf.Log(Random.value)) * Mathf.Sin(2f * Mathf.PI * Random.value));
    }
}
