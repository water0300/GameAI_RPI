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
        var MousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Ray ray = Camera.main.ScreenPointToRay(MousePos);
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(MousePos), Vector2.zero, Mathf.Infinity, ~(1 << 6 | 1 << 3));
        Character target;
        if(hit.collider.TryGetComponent<Character>(out target)){
            Debug.Log($"Hit {target.name}");
        }
        
    }

}
