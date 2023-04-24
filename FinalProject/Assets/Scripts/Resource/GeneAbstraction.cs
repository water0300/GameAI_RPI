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
        child.maxSpeed = LoadGene(_maxSpeed, 5, 1, 10);
        child.agentWanderForwardBias = LoadGene(_agentWanderForwardBias, .3f, 0, 1);
        child.foodYield = LoadGene(_foodYield, 25, 10, 90);
        child.detectionRadius = LoadGene(_detectionRadius, 7, 5, 35);
        child.metabolism = LoadGene(_metabolism, .3f, 0.3f, 1);
    }

    public float LoadFemaleGenes(){
        return HandleMutation(_gestationDuration, 5, GameManager.Instance.gestationDurationRange[0], GameManager.Instance.gestationDurationRange[1]);
    }

    public float LoadMaleGenes(){
       return HandleMutation(_desirability, .3f, 0, 1);
    }

    float LoadGene(Vector2 genePair, float maxMutationAmount, float minBound, float maxBound){
        float gene = Random.value > 0.5f ? genePair[0] : genePair[1];
        return HandleMutation(gene, maxMutationAmount, minBound, maxBound);
    }       

    float HandleMutation(float gene, float maxMutationAmount, float minBound, float maxBound){
        if(Random.value < GameManager.Instance.mutationChance){
            // Debug.Log("MUTATED");
            gene += Utility.RandomGaussian() * maxMutationAmount;
        }

        return Mathf.Clamp(gene, minBound, maxBound);
    }

}
