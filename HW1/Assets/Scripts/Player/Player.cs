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
        _rb.MovePosition(transform.position + _smoothedInputAxis.XZPlane() * MaxSpeed * Time.fixedDeltaTime);
    }


    private void FixedUpdate() {

            HandleWASDMovement();
    }





}
