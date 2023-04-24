using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Utility {
    
    public static LayerMask IgnoreLayer(int layerToIgnore){
        return ~(1 << layerToIgnore);
    }

    // public static bool CompareDistances

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

    public static Vector3 GetRandomNavmeshPosition(){
        //get a random position on the mesh
        NavMeshTriangulation navMeshTriangulation = NavMesh.CalculateTriangulation();



        int randomTriangleIndex = Random.Range(0, navMeshTriangulation.indices.Length / 3); // Select a random triangle
        Vector3[] vertices = navMeshTriangulation.vertices;
        int[] indices = navMeshTriangulation.indices;
        int index1 = indices[randomTriangleIndex * 3];
        int index2 = indices[randomTriangleIndex * 3 + 1];
        int index3 = indices[randomTriangleIndex * 3 + 2];
        Vector3 randomPoint = GetRandomPointInTriangle(vertices[index1], vertices[index2], vertices[index3]); // Get a random point within the triangle
        // _debugTrySpawnPoint = randomPoint;

        // NavMesh.get
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)){
            // A random position has been sampled successfully
            // _debugSpawnPoint = hit.position;
            return hit.position;
        }
        else
        {
            // Sampling failed, no valid position found
            Debug.LogError("Failed to sample a random position on the NavMesh.");
            return Vector3.zero;
        }
    }

    // Helper method to get a random point within a triangle
    static Vector3 GetRandomPointInTriangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        float r1 = Random.value;
        float r2 = Random.value;
        if (r1 + r2 >= 1.0f)
        {
            r1 = 1.0f - r1;
            r2 = 1.0f - r2;
        }
        float r3 = 1.0f - r1 - r2;
        return r1 * vertex1 + r2 * vertex2 + r3 * vertex3;
    }

}
