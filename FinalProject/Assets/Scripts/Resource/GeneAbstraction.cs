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

    public void LoadGenes(ref Animal child){
        child.maxSpeed = LoadGene(_maxSpeed);
        child.agentWanderForwardBias = LoadGene(_agentWanderForwardBias);
        child.foodYield = LoadGene(_foodYield);
        child.detectionRadius = LoadGene(_detectionRadius);
        child.metabolism = LoadGene(_metabolism);
    }

    public float LoadFemaleGenes(){
        return HandleMutation(_gestationDuration);
    }

    public float LoadMaleGenes(){
       return HandleMutation(_desirability);
    }

    float LoadGene(Vector2 genePair){
        float gene = Random.value > 0.5f ? genePair[0] : genePair[1];
        return HandleMutation(gene);
    }       

    float HandleMutation(float gene){
        if(Random.value < GameManager.Instance.mutationChance){
            Debug.Log("MUTATED");
            gene += Utility.RandomGaussian() * GameManager.Instance.mutationAmount;
        }

        return gene;
    }

}
