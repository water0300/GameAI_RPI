using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingWall : MonoBehaviour{
    public float distance = .5f;
    public float rate = 5f;
    private Rigidbody _rb;
    private void Start() {
        _rb = GetComponent<Rigidbody>();
    }
    public float counter = 0f;
    void FixedUpdate(){
        counter += Time.fixedDeltaTime*rate;
        _rb.MovePosition(_rb.position + Vector3.forward * distance * Mathf.Sin(counter));
    }
}
