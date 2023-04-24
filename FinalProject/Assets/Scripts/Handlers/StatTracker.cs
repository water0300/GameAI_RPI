using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StatTracker : MonoBehaviour
{
    // Start is called before the first frame update
    GameManager manager; //cache for perforamcne?

    public StatData dataSO;

    private string _overwriteFile;

    //stats
    int herbivorePopulationSize;
    // int 


    void Start(){
        manager = FindObjectOfType<GameManager>();
        dataSO.data.Clear();

        StartCoroutine(OnStatUpdate());

        
    }

    float timestep = 0;

    IEnumerator OnStatUpdate(){
        
        var population = manager.Population;
        while(true){
            dataSO.data.Add(new StatDataObj(timestep, population.Count));

            timestep += 1;

            yield return new WaitForSeconds(1f);

        }
        
    }
}
