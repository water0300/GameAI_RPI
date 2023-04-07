using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animal))]
public class AnimalSensor : MonoBehaviour
{
    public float detectionRadius;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
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
