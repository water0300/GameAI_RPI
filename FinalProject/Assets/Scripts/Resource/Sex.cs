using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Sex {
    
}

[System.Serializable]
public class Male : Sex {
    public float desirability; //range between 0-1
}

[System.Serializable]
public class Female : Sex {
    public float gestationDuration;
    public float minChance = 0.2f;
    private List<Male> _blacklist = new List<Male>();

    public bool RequestMate(Male male){
        if (_blacklist.Contains(male)){
            return false;
        }

        float chance = Mathf.Lerp(minChance, 1, male.desirability);
        if(Random.value > chance){
            _blacklist.Add(male);
            return false;
        } else {
            return true;
        }

    }

}