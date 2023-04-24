using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatData", menuName = "ScriptableObjects/StatData", order = 1)]
public class StatData : ScriptableObject
{
    public List<StatDataObj> data = new List<StatDataObj>();

}

[System.Serializable]
public class StatDataObj {
    public float timestep;
    public int populationCount;

    public StatDataObj(float timestep, int populationCount)
    {
        this.timestep = timestep;
        this.populationCount = populationCount;
    }
}
