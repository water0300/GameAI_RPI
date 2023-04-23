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

    [Header("Initial Conditions")]
    public int maleHerbivoreCount = 5;
    public int femaleHerbivoreCount = 5;

    [Header("Global Props")]
    public float lifetimeBoundSecs = 60;
    public float mutationChance = 0.05f;
    public float mutationAmount = 1;

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
    
    private void Start() {
        InitEnvironment();
    }

    private void InitEnvironment(){
        for(int i = 0; i < maleHerbivoreCount; ++i){
            Animal spawnedMale = Instantiate(maleHerbivore, Utility.GetRandomNavmeshPosition(), Quaternion.identity);
            RandomizeStats(ref spawnedMale);
            (spawnedMale.Sex as Male).desirability = Random.Range(0.4f, 1);
            Population.Add(spawnedMale);
        }

        for(int i = 0; i < femaleHerbivoreCount; ++i){
            Animal spawnedFemale = Instantiate(femaleHerbivore, Utility.GetRandomNavmeshPosition(), Quaternion.identity);
            RandomizeStats(ref spawnedFemale);
            (spawnedFemale.Sex as Female).gestationDuration = Random.Range(5f, 15f);
            Population.Add(spawnedFemale);
        }

    }

    private void RandomizeStats(ref Animal animal){
        animal.maxSpeed = Random.Range(2, 7);
        animal.agentWanderForwardBias = Random.Range(0.5f, 0.95f);
        animal.foodYield = Random.Range(30, 70);
        animal.detectionRadius = Random.Range(6, 18);
        animal.metabolism = Random.Range(0.5f, 1);
    }

    public void HandleBirth(Animal birthingAnimal, Female birthingSex){
        Animal child = Random.value > 0.5 ? Instantiate(maleHerbivore, birthingAnimal.transform.position, Quaternion.identity) : Instantiate(femaleHerbivore, birthingAnimal.transform.position, Quaternion.identity);

        birthingSex.GeneAbstraction.LoadGenes(ref child);
        if(child.Sex is Male){ //todo hard coded
            (child.Sex as Male).desirability = birthingSex.GeneAbstraction.LoadMaleGenes();
        } else {
            (child.Sex as Female).gestationDuration = birthingSex.GeneAbstraction.LoadFemaleGenes();
        }

        child.InfancyDuration = birthingSex.gestationDuration;

        Population.Add(child);
    }

    public void HandleDeath(Animal deadAnimal){
        Population.Remove(deadAnimal);
        Destroy(deadAnimal.gameObject);
    }
}
