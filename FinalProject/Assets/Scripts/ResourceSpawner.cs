using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class ResourceSpawner : MonoBehaviour {

    [Header("Prefab Refs")]
    public Plant resourcePrefab;

    [Header("Spawn Properties Refs")]
    [Range(10, 1000)] public int initialSpawnCount = 100;
    [Range(.1f, 5)] public float spawnRatePerSec = 1;
    [Range(0f, 1f)] public float spawnRateVariability = 0;
    [Range(.05f, 1f)] public float maxSpawnDelay = 0.05f;

    private List<Plant> _resourceList = new List<Plant>();

    private Vector3 _debugSpawnPoint;
    private Vector3 _debugTrySpawnPoint;

    private void Awake() {
    }

    private void Start() {
        for(int i = 0; i < initialSpawnCount; i++){
            Plant resource = Instantiate(resourcePrefab, GetRandomNavmeshPosition(), Quaternion.identity);
            _resourceList.Add(resource);
        }
        
        // StartCoroutine(SpawnResource());

    }

    IEnumerator SpawnResource(){
        while(true){

            //sample a position on the mesh
            Vector3 pos = GetRandomNavmeshPosition();

            float randoSpawnRate = Mathf.Max(Random.Range(spawnRatePerSec - spawnRateVariability, spawnRatePerSec + spawnRateVariability), maxSpawnDelay);
            // Debug.Log($"Rando Spawn Rate: {randoSpawnRate}");
            yield return new WaitForSeconds(1 / randoSpawnRate);
        }
    }

    

    public Vector3 GetRandomNavmeshPosition(){

        //get a random position on the mesh
        NavMeshTriangulation navMeshTriangulation = NavMesh.CalculateTriangulation();



        int randomTriangleIndex = Random.Range(0, navMeshTriangulation.indices.Length / 3); // Select a random triangle
        Vector3[] vertices = navMeshTriangulation.vertices;
        int[] indices = navMeshTriangulation.indices;
        int index1 = indices[randomTriangleIndex * 3];
        int index2 = indices[randomTriangleIndex * 3 + 1];
        int index3 = indices[randomTriangleIndex * 3 + 2];
        Vector3 randomPoint = GetRandomPointInTriangle(vertices[index1], vertices[index2], vertices[index3]); // Get a random point within the triangle
        _debugTrySpawnPoint = randomPoint;

        // NavMesh.get
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)){
            // A random position has been sampled successfully
            _debugSpawnPoint = hit.position;
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
    private Vector3 GetRandomPointInTriangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
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

    private void OnDrawGizmos() {
        if(_debugSpawnPoint != null)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_debugSpawnPoint, 1);

        if(_debugTrySpawnPoint != null)
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_debugTrySpawnPoint, 1.2f);
    }

}
