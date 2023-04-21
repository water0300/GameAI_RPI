using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Female : Sex {
    public float gestationDuration;
    [Range(0f, 1f)] public float minChance = 0.2f;
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