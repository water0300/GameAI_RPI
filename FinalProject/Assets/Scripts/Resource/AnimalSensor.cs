using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Animal))]
public class AnimalSensor : MonoBehaviour
{
    private Animal _animal;
    private int _floorLayerID;
    // Start is called before the first frame update
    void Awake(){
        _animal = GetComponent<Animal>();
        _floorLayerID = LayerMask.NameToLayer("Floor");
    }

    // Update is called once per frame
    void FixedUpdate() {
        if(_animal.ActiveState != null){
                _animal.Target = Physics.OverlapSphere(transform.position, _animal.detectionRadius, Utility.IgnoreLayer(_floorLayerID))?
            .FirstOrDefault(h => _animal.ActiveState.CompareGoalToTarget(h))?.transform;
        }
    }

    private void OnDrawGizmos() {
        if(_animal != null){
            Gizmos.color = Color.white;
            float deltaTheta = (2f*Mathf.PI) / 40f;
            float theta = 0f;
            Vector3 oldPos = transform.position;
            for(int i = 0; i < 40f; i++){
                Vector3 pos = new Vector3(_animal.detectionRadius * Mathf.Cos(theta), 0f, _animal.detectionRadius * Mathf.Sin(theta));
                Gizmos.DrawLine(oldPos, transform.position + pos);
                oldPos = transform.position + pos;
                theta += deltaTheta;
            }
        }

    }
}
