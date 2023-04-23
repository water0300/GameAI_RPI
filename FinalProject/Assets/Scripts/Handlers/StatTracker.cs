using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTracker : MonoBehaviour
{
    // Start is called before the first frame update
    GameManager manager; //cache for perforamcne?


    //stats
    int herbivorePopulationSize;
    // int 


    void Start(){
        manager = FindObjectOfType<GameManager>();
        StartCoroutine(OnStatUpdate());
        
    }

    IEnumerator OnStatUpdate(){
        while(true){
            var population = manager.Population;
        }
    }
}
