using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float forceAmount = 100f;

    private Vector3 _playerInput;
    private Rigidbody _rb;
    private GameManager manager;
   
    void Start(){   
        _rb = GetComponent<Rigidbody>();
        manager = FindObjectOfType<GameManager>();
    }


    void Update(){
        _playerInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized * -1f;
    }
    
    void FixedUpdate(){
        _rb.AddForce(_playerInput * forceAmount * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.layer == 3){
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            manager.FailRestart();
        } 
    }
}
