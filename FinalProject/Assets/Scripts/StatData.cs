using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatData", menuName = "ScriptableObjects/StatData", order = 1)]
public class StatData : ScriptableObject
{
    public List<StatDataObj> herbivoreData = new List<StatDataObj>();
    public List<StatDataObj> carnivoreData = new List<StatDataObj>();

}

[System.Serializable]
public class StatDataObj {
    public float timestep;
    public int populationCount;
    public float avgMaxSpeed;
    public float averageForwardBias;
    public float avgMetabolism;
    public float avgDetectionRadius;
    public float avgDesire;
    public float avgGestationDuration;

    public StatDataObj(float timestep, int populationCount, float avgMaxSpeed, float averageForwardBias, float avgMetabolism, float avgDetectionRadius, float avgDesire, float avgGestationDuration)
    {
        this.timestep = timestep;
        this.populationCount = populationCount;
        this.avgMaxSpeed = avgMaxSpeed;
        this.averageForwardBias = averageForwardBias;
        this.avgMetabolism = avgMetabolism;
        this.avgDetectionRadius = avgDetectionRadius;
        this.avgDesire = avgDesire;
        this.avgGestationDuration = avgGestationDuration;

    }
}
