using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour {
    
    [Header("Movement Attributes")]
    public float maxSpeed;
    public float detectionRadius;

    [Header("Health Attributes")]
    public float maxThirst;
    public float maxHunger;

    [field: SerializeField] public float CurrentThirst {get; private set; }
    [field: SerializeField] public float CurrentHunger {get; private set; }

    private void OnDrawGizmos() {
        float deltaTheta = (2f*Mathf.PI) / 40f;
        float theta = 0f;
        Vector3 oldPos = transform.position;
        for(int i = 0; i < 40f; i++){
            Vector3 pos = new Vector3(detectionRadius * Mathf.Cos(theta), 0f, detectionRadius * Mathf.Sin(theta));
            Gizmos.DrawLine(oldPos, transform.position + pos);
            oldPos = transform.position + pos;
            theta += deltaTheta;
        }
    }

}
