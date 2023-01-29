using System;
using System.Linq;
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
    private Vector2 _smoothedInputAxis;
    private Vector2 _smoothedInputVelocity;
    private void Awake() {
        _rb = GetComponent<Rigidbody>();
    }
    private void Start() {
        OnTargetActivate?.Invoke(_rb);
    }


    RaycastHit hit;
    private void MovePlayer(Vector3 correctedInputAxis){
        _smoothedInputAxis = Vector2.SmoothDamp(_smoothedInputAxis, correctedInputAxis, ref _smoothedInputVelocity, 1f - AccelerationFactor);
        _rb.MovePosition(transform.position + _smoothedInputAxis.XZPlane() * MaxSpeed * Time.fixedDeltaTime);
    }

    float avgDot;
    private void FixedUpdate() {
        if(_isColliding){
            // Debug.Log($"normal: {avgNormal} dot: {Vector3.Dot(avgNormal, InputAxis.XZPlane())}");
            avgDot = Vector3.Dot(avgNormal, InputAxis.XZPlane());
            if(avgDot < 0){ //scale movement based on dot (-1 == no movmeent, -0.01 == some sideways movement)
                Vector3 tangent = Vector3.Cross( avgNormal, Vector3.up); //respect to y axis
                MovePlayer(tangent.ExcludeY() * Vector3.Dot(tangent, InputAxis.XZPlane()));
            } else {
                MovePlayer(InputAxis);
            }
        } else {
            MovePlayer(InputAxis);
        }
        
    }

    //todo - move to superclass of agent/player since code is duplicated
    //todo - corners are screwed
    //get average contact point normal
    //only allow movement that is dot product > 0 (i.e away from collider lol)
    private Vector3 avgNormal;
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.layer == 6){
            return;
        }

        ContactPoint[] contacts = new ContactPoint[other.contactCount];
        avgNormal = Vector3.zero;
        int contactCount = other.GetContacts(contacts);
        foreach(var c in contacts){
            avgNormal += c.normal * contactCount;
        }
        avgNormal.Normalize();
        _isColliding = true;
    } 
    private void OnCollisionExit(Collision other) {
        if(other.gameObject.layer == 6){
            return;
        }
        _isColliding = false;
    }




}
