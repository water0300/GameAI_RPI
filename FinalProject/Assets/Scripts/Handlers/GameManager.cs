using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour {
    //singleton stuff
    private static GameManager instance;
    public static GameManager Instance {
        get { return instance; }
    }

    [Header("Prefabs")]
    public Herbivore maleHerbivore;
    public Herbivore femaleHerbivore;
    public Carnivore maleCarnivore;
    public Carnivore femaleCarnivore;

    [Header("Initial Conditions")]
    public int maleHerbivoreCount = 5;
    public int femaleHerbivoreCount = 5;
    public int maleCarnivoreCount = 5;
    public int femaleCarnivoreCount = 5;

    [Header("Global Props")]
    public bool debug;
    public float lifetimeBoundSecs = 60;
    public float mutationChance = 0.05f;
    public float mutationAmount = 1;
    public Vector2 gestationDurationRange = new Vector2(10, 30);
    public Vector2 litterSizeRandomRange = new Vector2(3, 12);
    public Vector2 pregnancyDurationRandomRangeSecs = new Vector2(5, 15);
    [Range(0f, 1f)] public float acceptMaleMinChance = 0.4f;

    [Range(1f, 50f)] public float timeScale = 1;

    private CinemachineVirtualCamera vcam;

    // [field: SerializeField] public List<Animal> Population {get; private set; } = new List<Animal>();
    [field: SerializeField] public List<Herbivore> HerbivorePop {get; private set; } = new List<Herbivore>();
    [field: SerializeField] public List<Carnivore> CarnivorePop {get; private set; } = new List<Carnivore>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        vcam = FindObjectOfType<CinemachineVirtualCamera>();
    }
    
    private void Start() {
        if(!debug)
            InitEnvironment();
    }

    private void Update() {

        if(Input.GetKeyDown(KeyCode.Space)){
            vcam.Follow = HerbivorePop[Random.Range(0, HerbivorePop.Count)].transform;

        }

        if(Input.GetKeyDown(KeyCode.X)){
            vcam.Follow = CarnivorePop[Random.Range(0, CarnivorePop.Count)].transform;

        }
        Time.timeScale = timeScale;
    }

    int id = 0;

    private void InitEnvironment(){
        for(int i = 0; i < maleHerbivoreCount; ++i){
            Animal spawnedMale = Instantiate(maleHerbivore, Utility.GetRandomNavmeshPosition(), Quaternion.identity);
            RandomizeStats(ref spawnedMale);
            (spawnedMale.Sex as Male).desirability = Random.Range(0.4f, 1);
            spawnedMale.InfancyDuration = 20;
            spawnedMale.Age = Random.Range(0, 20);
            HerbivorePop.Add(spawnedMale as Herbivore);
            spawnedMale.name += $"{id}";
            id++;
        }

        for(int i = 0; i < femaleHerbivoreCount; ++i){
            Animal spawnedFemale = Instantiate(femaleHerbivore, Utility.GetRandomNavmeshPosition(), Quaternion.identity);
            RandomizeStats(ref spawnedFemale);
            (spawnedFemale.Sex as Female).gestationDuration = Random.Range(gestationDurationRange[0], gestationDurationRange[1]);
            spawnedFemale.InfancyDuration = 20;
            spawnedFemale.Age = Random.Range(0, 20);
            HerbivorePop.Add(spawnedFemale as Herbivore);
            spawnedFemale.name += $"{id}";
            id++;
        }

        for(int i = 0; i < maleCarnivoreCount; ++i){
            Animal spawnedMale = Instantiate(maleCarnivore, Utility.GetRandomNavmeshPosition(), Quaternion.identity);
            RandomizeStats(ref spawnedMale);
            (spawnedMale.Sex as Male).desirability = Random.Range(0.4f, 1);
            spawnedMale.InfancyDuration = 20;
            spawnedMale.Age = Random.Range(0, 20);
            CarnivorePop.Add(spawnedMale as Carnivore);
            spawnedMale.name += $"{id}";
            id++;
        }

        for(int i = 0; i < femaleCarnivoreCount; ++i){
            Animal spawnedFemale = Instantiate(femaleCarnivore, Utility.GetRandomNavmeshPosition(), Quaternion.identity);
            RandomizeStats(ref spawnedFemale);
            (spawnedFemale.Sex as Female).gestationDuration = Random.Range(gestationDurationRange[0], gestationDurationRange[1]);
            spawnedFemale.InfancyDuration = 20;
            spawnedFemale.Age = Random.Range(0, 20);
            CarnivorePop.Add(spawnedFemale as Carnivore);
            spawnedFemale.name += $"{id}";
            id++;
        } 

    }

    private void RandomizeStats(ref Animal animal){
        animal.maxSpeed = Random.Range(2, 6);
        animal.agentWanderForwardBias = Random.Range(0.5f, 0.95f);
        // animal.foodYield = Random.Range(30, 70);
        animal.detectionRadius = Random.Range(6, 28);
        animal.metabolism = Random.Range(0.5f, 1);
    }

    public void HandleBirth(Animal birthingAnimal, Female birthingSex){
        if(birthingSex == null){
            Debug.LogWarning("BIRTHING SEX MISSING???");
            return;
        } else if (birthingSex.GeneAbstraction == null){
            Debug.LogWarning("GENE ABSTRACTION MISSING???");
            return;
        } 

        Animal child = null;

        if(birthingAnimal is Carnivore){
            child = Random.value > 0.5 ? Instantiate(maleCarnivore, birthingAnimal.transform.position, Quaternion.identity) : Instantiate(femaleCarnivore, birthingAnimal.transform.position, Quaternion.identity);
        } else if (birthingAnimal is Herbivore) {
            child = Random.value > 0.5 ? Instantiate(maleHerbivore, birthingAnimal.transform.position, Quaternion.identity) : Instantiate(femaleHerbivore, birthingAnimal.transform.position, Quaternion.identity);
        } else {
            Debug.LogError("what u give birth to tf?");
        }


        birthingSex.GeneAbstraction.LoadGenes(ref child);
        if(child.Sex is Male){ //todo hard coded
            (child.Sex as Male).desirability = birthingSex.GeneAbstraction.LoadMaleGenes();
        } else {
            (child.Sex as Female).gestationDuration = birthingSex.GeneAbstraction.LoadFemaleGenes();
        }

        child.InfancyDuration = birthingSex.gestationDuration;

        if(child is Herbivore){
            HerbivorePop.Add(child as Herbivore);

        }else if(child is Carnivore){
            // Debug.Log("Carnivore gave birth");
            CarnivorePop.Add(child as Carnivore);

        } else {
            Debug.LogError("tf is this child bro");
        }

            child.name += $"{id}";
            id++;

    }

    public void HandleDeath(Animal deadAnimal, string causeOfDeath){
        string typ = "";
        if(deadAnimal is Herbivore){
            typ = "herbivore";
            HerbivorePop.Remove(deadAnimal as Herbivore);

        }else if(deadAnimal is Carnivore){
            typ = "carnivore";
            CarnivorePop.Remove(deadAnimal as Carnivore);

        } else {
            Debug.LogError("tf is this child bro");
        }
        Debug.Log($"{typ}, Cause of Death: {causeOfDeath}");
        Destroy(deadAnimal.gameObject);
    }
}
