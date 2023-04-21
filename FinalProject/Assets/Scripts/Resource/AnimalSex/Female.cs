using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Female : Sex {
    public Vector2 litterSizeRandomRange = new Vector2(1, 12);
    public Vector2 pregnancyDurationRandomRangeSecs = new Vector2(5, 15);
    public float gestationDuration; //variable
    [Range(0f, 1f)] public float minChance = 0.2f;

    [field: SerializeField] public bool IsPregnant {get; set; }
    [field: SerializeField] public GeneAbstraction GeneAbstraction {get; set; }
    private List<Male> _blacklist = new List<Male>();
    private Animal _animal;

    private void Awake() {
        _animal = GetComponent<Animal>();
    }

    public void InitPregnancyHandler() => StartCoroutine(PregnancyHandler());

    private IEnumerator PregnancyHandler() {
        yield return new WaitForSeconds(Random.Range(pregnancyDurationRandomRangeSecs[0], pregnancyDurationRandomRangeSecs[1]));

        // if(_animal.IsAlive){ //safety check???
        DoBirth();

        IsPregnant = false;
    }

    private void DoBirth(){

        for(int i = 0; i < Random.Range(litterSizeRandomRange[0], litterSizeRandomRange[1]); i++){

        }

        GeneAbstraction = null;

    }

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