using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    
    public float speed = 5f;

    // Update is called once per frame
    void Update() {
        float hoz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        transform.position += new Vector3(hoz * Time.deltaTime * speed, 0f, vert * Time.deltaTime * speed);

    }
}
