using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
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
        dataSO.herbivoreData.Clear();
        dataSO.carnivoreData.Clear();

        StartCoroutine(OnStatUpdate());

        
    }

    float timestep = 0;

    public float getAvgMaxSpeed(bool type){
        float avgVal = 0;
        if(type){
            avgVal = manager.HerbivorePop.Select(h => h.maxSpeed).Average();
        } else {
            avgVal = manager.CarnivorePop.Select(c => c.maxSpeed).Average();
        }

        return avgVal;
    }
    public float getAverageForwardBias(bool type){
        float avgVal = 0;
        if(type){
            avgVal = manager.HerbivorePop.Select(h => h.agentWanderForwardBias).Average();
        } else {
            avgVal = manager.CarnivorePop.Select(c => c.agentWanderForwardBias).Average();
        }

        return avgVal;
    }
    public float getAvgMetabolism(bool type){
        float avgVal = 0;
        if(type){
            avgVal = manager.HerbivorePop.Select(h => h.metabolism).Average();
        } else {
            avgVal = manager.CarnivorePop.Select(c => c.metabolism).Average();
        }

        return avgVal;
    }
    // public float getAvgFoodYield(bool type){
    //     float avgVal = 0;
    //     if(type){
    //         avgVal = manager.HerbivorePop.Select(h => h.foodYield).Average();
    //     } else {
    //         avgVal = manager.CarnivorePop.Select(c => c.foodYield).Average();
    //     }

    //     return avgVal;
    // }
    public float getAvgDetectionRadius(bool type){
        float avgVal = 0;
        if(type){
            avgVal = manager.HerbivorePop.Select(h => h.detectionRadius).Average();
        } else {
            avgVal = manager.CarnivorePop.Select(c => c.detectionRadius).Average();
        }

        return avgVal;
    }
    public float getAvgDesire(bool type){
        float avgVal = 0;
        if(type){
            avgVal = manager.HerbivorePop.Where(h => h.Sex is Male).Select(h => (h.Sex as Male).desirability).Average();
        } else {
            avgVal = manager.CarnivorePop.Where(h => h.Sex is Male).Select(h => (h.Sex as Male).desirability).Average();
        }

        return avgVal;
    }
    public float getAvgGestationDuration(bool type){
        float avgVal = 0;
        if(type){
            avgVal = manager.HerbivorePop.Where(h => h.Sex is Female).Select(h => (h.Sex as Female).gestationDuration).Average();
        } else {
            avgVal = manager.CarnivorePop.Where(h => h.Sex is Female).Select(h => (h.Sex as Female).gestationDuration).Average();
        }
        return avgVal;
    }



    IEnumerator OnStatUpdate(){
        
        while(manager.HerbivorePop.Count > 0 && manager.CarnivorePop.Count > 0){
            dataSO.herbivoreData.Add(new StatDataObj(
                timestep, 
                manager.HerbivorePop.Count,
                getAvgMaxSpeed(true),
                getAverageForwardBias(true),
                getAvgMetabolism(true),
                // getAvgFoodYield(true),
                getAvgDetectionRadius(true),
                getAvgDesire(true),
                getAvgGestationDuration(true)

            ));
            dataSO.carnivoreData.Add(new StatDataObj(
                timestep, manager.CarnivorePop.Count,
                getAvgMaxSpeed(false),
                getAverageForwardBias(false),
                getAvgMetabolism(false),
                // getAvgFoodYield(false),
                getAvgDetectionRadius(false),
                getAvgDesire(false),
                getAvgGestationDuration(false)
            ));

            timestep += 1;

            yield return new WaitForSeconds(1f);

        }
        
    }
}
