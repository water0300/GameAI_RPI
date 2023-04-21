using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneAbstraction {
    private Vector2 _maxSpeed;
    private Vector2 _agentWanderForwardBias;
    private Vector2 _foodYield;
    private Vector2 _detectionRadius;
    private Vector2 _metabolism; 


    //sex specific
    private float _gestationDuration;
    private float _desirability;

    public GeneAbstraction(Animal male, Animal female){
        if(male.Sex is Female || female.Sex is Male){
            Debug.LogError("Sex assignment problem");
        }

        _maxSpeed = new Vector2(male.maxSpeed, female.maxSpeed);
        _agentWanderForwardBias = new Vector2(male.agentWanderForwardBias, female.agentWanderForwardBias);
        _foodYield = new Vector2(male.foodYield, female.foodYield);
        _detectionRadius = new Vector2(male.detectionRadius, female.detectionRadius);
        _metabolism  = new Vector2(male.metabolism, female.metabolism);

        _gestationDuration = (female.Sex as Female).gestationDuration;
        _desirability = (male.Sex as Male).desirability;
    }



}
