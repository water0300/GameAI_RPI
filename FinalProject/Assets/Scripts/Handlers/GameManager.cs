using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    //singleton stuff
    private static GameManager instance;
    public static GameManager Instance {
        get { return instance; }
    }

    [Header("Prefabs")]
    public Herbivore maleHerbivore;
    public Herbivore femaleHerbivore;

    [Header("Global Props")]
    public float mutationChance = 0.05f;
    public float mutationAmount = 5;

    [field: SerializeField] public List<Animal> Population {get; private set; } = new List<Animal>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void HandleBirth(Animal birthingAnimal, Female birthingSex){
        Animal child = Random.value > 0.5 ? Instantiate(maleHerbivore, birthingAnimal.transform.position, Quaternion.identity) : Instantiate(femaleHerbivore, birthingAnimal.transform.position, Quaternion.identity);

        birthingSex.GeneAbstraction.LoadGenes(ref child);
        if(child.Sex is Male){ //todo hard coded
            (child.Sex as Male).desirability = birthingSex.GeneAbstraction.LoadMaleGenes();
        } else {
            (child.Sex as Female).gestationDuration = birthingSex.GeneAbstraction.LoadFemaleGenes();
        }

        Population.Add(child);
    }

    void HandleDeath(){

    }
}
