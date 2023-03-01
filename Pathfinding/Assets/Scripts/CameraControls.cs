using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    private Camera _camera;
    public float speed = 3f;
    public float scrollSpeed = 3f;
    private void Start() {
        _camera = GetComponent<Camera>();
    }

    private void Update() {
        Vector2 _inputAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        
        transform.position += _inputAxis.XYPlane() * speed * Time.deltaTime;

        _camera.orthographicSize -= Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime;
    }
}
