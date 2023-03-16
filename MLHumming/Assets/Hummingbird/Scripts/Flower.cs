using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [Tooltip("Color when flower full")]
    public Color fullFlowerColor = new Color(1f, 0f, 0.3f);
    
    [Tooltip("Color when flower empty")]
    public Color emptyColorFlower = new Color(.5f, 0f, 1f);
    
    [HideInInspector]
    public Collider nectarCollider;

    private Collider flowerCollider;
    private Material flowerMaterial;

    public Vector3 FlowerUpVector { get => nectarCollider.transform.up; }
    public Vector3 FlowerCenterPosition { get => nectarCollider.transform.position; }

    public float NectarAmount {get; private set; }
    public bool HasNectar { get => NectarAmount > 0f; }

    public float Feed(float amount){
        float nectarTaken = Mathf.Clamp(amount, 0f, NectarAmount);

        NectarAmount -= amount;

        if(NectarAmount <= 0){
            NectarAmount = 0;

            flowerCollider.gameObject.SetActive(false);
            nectarCollider.gameObject.SetActive(false);

            flowerMaterial.SetColor("_BaseColor", emptyColorFlower);

        }

        return nectarTaken;
    }

    public void ResetFlower(){
        NectarAmount = 1f;

        flowerCollider.gameObject.SetActive(true);
        nectarCollider.gameObject.SetActive(true);

        flowerMaterial.SetColor("_BaseColor", fullFlowerColor);
    }

    private void Awake(){
        flowerMaterial = GetComponent<MeshRenderer>().material;
        flowerCollider = transform.Find("FlowerCollider").GetComponent<Collider>();
        nectarCollider = transform.Find("FlowerNectarCollider").GetComponent<Collider>();
    }
}
