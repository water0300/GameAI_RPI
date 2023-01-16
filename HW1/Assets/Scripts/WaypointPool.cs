// using System;
// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class WaypointPool : MonoBehaviour {
//     [Header("In Scene References")]
//     public Player player;
//     [Header("Prefab References")]
//     public Waypoint waypointPrefab;

//     [Header("Options")]
//     public int maxWaypoints = 3;
//     public float waypointYOffset = 1f;
//     public event Action<GameObject> OnWaypointSelect;
//     private Queue<Waypoint> _activePool = new Queue<Waypoint>();
//     private Queue<Waypoint> _pendingPool = new Queue<Waypoint>();


//     private void OnEnable() {
//         player.OnWaypointSpawn += SpawnWaypoint;
//     }

//     private void OnDisable() {
//         player.OnWaypointSpawn -= SpawnWaypoint;
//     }

//     public void SpawnWaypoint(Vector3 position) {
//         if(_activePool.Count == maxWaypoints){
//             Requeue(_activePool.Dequeue(), position + Vector3.up * waypointYOffset);
//             OnWaypointSelect?.Invoke(_activePool.Peek().gameObject);
//         } else if(_pendingPool.Count != 0){
//             ReactivateFromPool(_pendingPool.Dequeue(), position + Vector3.up * waypointYOffset);
//         } else {
//             _activePool.Enqueue(Instantiate(waypointPrefab, position + Vector3.up * waypointYOffset, Quaternion.identity));
//             if(_activePool.Count == 1){
//                 OnWaypointSelect?.Invoke(_activePool.Peek().gameObject);
//             }
//         }

//     }

//     public void OnWaypointReached() {
//         ReturnToPool(_activePool.Dequeue());
//         if(_activePool.TryPeek(out Waypoint nextWp)){
//             OnWaypointSelect?.Invoke(nextWp.gameObject);
//         } else {
//             OnWaypointSelect?.Invoke(null);
//         }
//     }

//     void ReturnToPool(Waypoint deactivatedWp){
//         deactivatedWp.gameObject.SetActive(false);
//         _pendingPool.Enqueue(deactivatedWp);
//     }
//     void Requeue(Waypoint reactivatedWp, Vector3 position){
//         reactivatedWp.transform.position = position;
//         _activePool.Enqueue(reactivatedWp);
//     }
//     void ReactivateFromPool(Waypoint reactivatedWp, Vector3 position){
//         reactivatedWp.gameObject.SetActive(true);
//         Requeue(reactivatedWp, position);
//     }


}
