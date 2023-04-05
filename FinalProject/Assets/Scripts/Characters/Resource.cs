using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {
    
    public bool isInfinite = false;
    [Range(0f, 500f)] public float maxCapacity;
    [Range(0f, 10f)] public float regenerationPerSec; 
    public float CurrentCapacity {get; private set; }
    public bool IsDead {get; private set; }
    public bool IsEmpty{get => CurrentCapacity <= 0f; }

    private void Start() {
        if(!isInfinite){
            StartCoroutine(DoRegeneration());
        }
    }

    IEnumerator DoRegeneration(){
        while(!IsDead){
            CurrentCapacity = Mathf.Min(CurrentCapacity + regenerationPerSec, maxCapacity);
            yield return new WaitForSeconds(1f);
        }
    }

    public float GetConsumed(float amount){
        if(isInfinite){
            return amount;
        }

        float resourceTaken = Mathf.Clamp(amount, 0f, CurrentCapacity);
        CurrentCapacity -= resourceTaken;

        if(CurrentCapacity == 0){
            //do something to indicate shit is done
        }

        return resourceTaken;
    }
    
}
