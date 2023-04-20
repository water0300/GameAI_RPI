using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Resource {
    
    [Range(0f, 500f)] public float maxCapacity;
    [Range(0f, 10f)] public float regenerationPerSec; 
    public float CurrentCapacity {get; private set; }
    public bool IsDead {get; private set; }
    public bool IsEmpty{get => CurrentCapacity <= 0f; }

    private void Start() {
        StartCoroutine(DoRegeneration());
    }

    IEnumerator DoRegeneration(){
        while(!IsDead){
            CurrentCapacity = Mathf.Min(CurrentCapacity + regenerationPerSec, maxCapacity);
            yield return new WaitForSeconds(1f);
        }
    }

    public override float GetConsumed(float amount){
        float resourceTaken = Mathf.Clamp(amount, 0f, CurrentCapacity);
        CurrentCapacity -= resourceTaken;

        if(CurrentCapacity == 0){
            //do something to indicate shit is done
        }

        return resourceTaken;
    }
    
}
