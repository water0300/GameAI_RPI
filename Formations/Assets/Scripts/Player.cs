using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    public float speed = 3f;
    public float rotSpeed = 2f;
    public Vector2 InputAxis {get; set;}
    public float RotationAxis {get; set;}
    private Rigidbody2D _rb;
    private FormationManager _fm;
    void Start(){
        _rb = GetComponent<Rigidbody2D>();
        _fm = FindObjectOfType<FormationManager>();
    }

    // Update is called once per frame
    void LateUpdate(){
        _rb.MovePosition(_rb.position + InputAxis * speed * Time.fixedDeltaTime);
        // _rb.MoveRotation()
        // transform.eulerAngles += Vector3.forward * RotationAxis * rotSpeed * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log(other.gameObject.name);
        Character target;
        // Debug.Log(worldpos);

        if(other.gameObject.TryGetComponent<Character>(out target)){
            Debug.Log($"Hit {target.name}");
            _fm.RemoveCharacter(target);
            target.gameObject.SetActive(false);
        } 
    }

    public void OnShoot(){

        // Vector3 mousePos = Mouse.current.position.ReadValue();   
        // Vector3 worldpos=Camera.main.ScreenToWorldPoint(mousePos);
        // worldpos.z = -10f;

        // RaycastHit2D hit = Physics2D.Raycast(worldpos, worldpos - Vector3.up, Mathf.Infinity, ~(1 << 6 | 1 << 3));
        // // RaycastHit2D hit = Physics2D.Raycast(Camera.main.transform.position, Camera.main.transform.position - worldpos);
        // // Character target;
        // // Debug.Log(worldpos);

        // //     if(hit.collider != null && hit.collider.TryGetComponent<Character>(out target)){
        // //     Debug.Log($"Hit {target.name}");
        // // } 
        // if(hit.collider != null){
        //     Debug.Log($"Hit {hit.collider.name}");
        // } else {
        //     Debug.Log("Miss");
        // }

        //     if(hit.collider != null && hit.collider.TryGetComponent<Character>(out target)){
        //     Debug.Log($"Hit {target.name}");
        // } 
        
    }

}
