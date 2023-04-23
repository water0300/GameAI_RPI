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
            Plant resource = Instantiate(resourcePrefab, Utility.GetRandomNavmeshPosition(), Quaternion.identity);
            _resourceList.Add(resource);
        }
        
        // StartCoroutine(SpawnResource());

    }

    IEnumerator SpawnResource(){
        while(true){

            //sample a position on the mesh
            Vector3 pos = Utility.GetRandomNavmeshPosition();

            float randoSpawnRate = Mathf.Max(Random.Range(spawnRatePerSec - spawnRateVariability, spawnRatePerSec + spawnRateVariability), maxSpawnDelay);
            // Debug.Log($"Rando Spawn Rate: {randoSpawnRate}");
            yield return new WaitForSeconds(1 / randoSpawnRate);
        }
    }

   

    private void OnDrawGizmos() {
        // if(_debugSpawnPoint != null)
        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawWireSphere(_debugSpawnPoint, 1);

        // if(_debugTrySpawnPoint != null)
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawWireSphere(_debugTrySpawnPoint, 1.2f);
    }

}
