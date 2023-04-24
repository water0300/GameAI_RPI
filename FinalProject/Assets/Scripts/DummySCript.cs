using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySCript : MonoBehaviour
{
   AnimalBehaviorState ActiveState;

    //Animal Superclass

    void Update()
    {
        UpdateResourceValues();

        //if goal changed
        if(DecideGoal())
        { 
            //...
        }
        ActiveState.OnUpdate(); 
    }

    void UpdateResourceValues(){}
    bool DecideGoal() => true;
}

public class Chromosome
{

    public Chromosome(Animal mother, Animal father)
    {
        _maxSpeed = new Vector2(mother.maxSpeed, father.maxSpeed);
        _detectionRadius = new Vector2(mother.detectionRadius, father.detectionRadius);
        //...


    }

    public void LoadGenes(ref Animal child, float maxMutationAmount) //crossover
    {
        float gene = 0;
        //...
        if(Random.value < 0.05f){
            gene += Utility.RandomGaussian() * maxMutationAmount;
        }


    }

    float HandleMutation(float gene, float maxMutationAmount, float minBound, float maxBound){
        if(Random.value < 0.05f){
            // Debug.Log("MUTATED");
            gene += Utility.RandomGaussian() * maxMutationAmount;
        }

        return Mathf.Clamp(gene, minBound, maxBound);
    }

        Vector2 _maxSpeed;
    Vector2 _detectionRadius;
}
