using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {
    [field: SerializeField] public float MaxSpeed {get; set; } = 5f;
    [field: SerializeField] public float AccelerationFactor {get; set; } = .2f;
    public event Action<Rigidbody> OnTargetActivate;
    private Rigidbody _rb;
    public Vector2 InputAxis {get; set;}
    private bool _isColliding = false;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
    }
    private void Start() {
        OnTargetActivate?.Invoke(_rb);
    }


    private Vector2 _smoothedInputAxis;
    private Vector2 _smoothedInputVelocity;
    private void HandleWASDMovement(){
        _smoothedInputAxis = Vector2.SmoothDamp(_smoothedInputAxis, InputAxis, ref _smoothedInputVelocity, 1f - AccelerationFactor);
        // if(_isColliding){
        //     return;
        // }
        _rb.MovePosition(transform.position + _smoothedInputAxis.XZPlane() * MaxSpeed * Time.fixedDeltaTime);
    }


    private void FixedUpdate() {
        HandleWASDMovement();
    }

    //todo - move to superclass of agent/player since code is duplicated
    //todo - ignore floor
    private void OnCollisionEnter(Collision other) {
        Debug.Log("enter");
        _isColliding = true;
    } 
    private void OnCollisionExit(Collision other) {
        Debug.Log("exit");
        _isColliding = false;
    }




}
