using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    public float speed = 3f;
    public float rotSpeed = 2f;
    public Vector2 InputAxis {get; set;}
    public float RotationAxis {get; set;}

    // Update is called once per frame
    void Update(){
        transform.position += InputAxis.XYPlane() * speed * Time.deltaTime;
        transform.eulerAngles += Vector3.forward * RotationAxis * rotSpeed * Time.deltaTime;
    }

    public void OnShoot(){

        Vector3 mousePos = Mouse.current.position.ReadValue();   
        Vector3 worldpos=Camera.main.ScreenToWorldPoint(mousePos);
        worldpos.z = 10f;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.transform.position, Camera.main.transform.position - worldpos, Mathf.Infinity, ~(1 << 6 | 1 << 3));
        // RaycastHit2D hit = Physics2D.Raycast(Camera.main.transform.position, Camera.main.transform.position - worldpos);
        Character target;
        // Debug.Log(worldpos);
            if(hit.collider != null && hit.collider.TryGetComponent<Character>(out target)){
            Debug.Log($"Hit {target.name}");
        } 
        
    }

}
