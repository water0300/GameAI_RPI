using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Female : Sex {
    
    public float gestationDuration; //variable

    [field: SerializeField] public bool IsPregnant {get; private set; }
    [field: SerializeField] public GeneAbstraction GeneAbstraction {get; set; }
    [SerializeField] private List<Male> _blacklist = new List<Male>();
    private Animal _animal;

    private void Awake() {
        _animal = GetComponent<Animal>();
    }

    public void InitPregnancyHandler() => StartCoroutine(PregnancyHandler());

    private IEnumerator PregnancyHandler() {
        IsPregnant = true;
        yield return new WaitForSeconds(Random.Range(GameManager.Instance.pregnancyDurationRandomRangeSecs[0], GameManager.Instance.pregnancyDurationRandomRangeSecs[1]));

        for(int i = 0; i < Random.Range(GameManager.Instance.litterSizeRandomRange[0], GameManager.Instance.litterSizeRandomRange[1]); i++){
            // Instantiate(_animal.infant)
            if(!_animal.IsAlive){
                break;
            }

            GameManager.Instance.HandleBirth(_animal, this);
            yield return new WaitForSeconds(0.1f);
        }

        GeneAbstraction = null;

        IsPregnant = false;
    }

    private void DoBirth(){

        Debug.Log("BIRTHING");


    }

    public bool RequestMate(Male male){
        if (_blacklist.Contains(male)){
            return false;
        }

        float chance = Mathf.Lerp(GameManager.Instance.acceptMaleMinChance, 1, male.desirability);
        if(Random.value > chance){
            _blacklist.Add(male);
            return false;
        } else {
            return true;
        }

    }

}