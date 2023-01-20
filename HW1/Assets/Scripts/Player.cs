using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {
    public float maxSpeed = 20f;
    public float _smoothedInputSpeed = .2f;
    // public event Action<Vector3> OnWaypointSpawn;
    public event Action<Rigidbody> OnTargetActivate;
    // public event Action<Vector3> OnTargetMove;
    private Rigidbody _rb;
    public Vector2 InputAxis {get; set;}
    // public Vector2 MouseAxis {get; set;}
    // public bool IsMouseControl {get; set; } = false;
    private void Awake() {
        _rb = GetComponent<Rigidbody>();
    }
    private void Start() {
        OnTargetActivate?.Invoke(_rb);
    }

    // //mouse movement character
    // private void HandleMouseMovement(){
    //     Ray ray = Camera.main.ScreenPointToRay(MouseAxis);
    //     RaycastHit hit;
    //     // Debug.Log("asdf");

    //     if(Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("World"))){
    //         _rb.MovePosition(InputAxis);
    //     }
    // }

    private Vector2 _smoothedInputAxis;
    private Vector2 _smoothedInputVelocity;
    private void HandleWASDMovement(){
        // Debug.Log("InputAxis");
        _smoothedInputAxis = Vector2.SmoothDamp(_smoothedInputAxis, InputAxis, ref _smoothedInputVelocity, _smoothedInputSpeed);
        _rb.MovePosition(transform.position + _smoothedInputAxis.XZPlane() * maxSpeed * Time.fixedDeltaTime);
    }


    private void FixedUpdate() {
        // if(IsMouseControl){
        //     HandleMouseMovement();
        // } else {
            HandleWASDMovement();
        // }
    }

    // public void IssueCommand(){
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     RaycastHit hit;
    //     if(Physics.Raycast(ray, out hit, Mathf.Infinity, ~6)){
    //         Debug.Log(hit.transform.gameObject.layer);
    //         OnWaypointSpawn?.Invoke(hit.point);
    //     }

    // }



}
