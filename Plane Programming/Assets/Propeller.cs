using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour
{
    // Update is called once per frame
    float rotFactor = 1000f;
    void Update(){
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + rotFactor * Time.deltaTime);
    }
}
