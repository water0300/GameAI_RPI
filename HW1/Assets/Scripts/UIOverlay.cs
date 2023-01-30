using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIOverlay : MonoBehaviour {

    public GameObject PursueUI;
    public GameObject WanderUI;
    public GameObject PathingUI;

    public Player player;
    public Agent agent;
    private void OnEnable() {
        agent.OnStateChange += OnStateChange;
    }
    
    private void OnDisable() {
        agent.OnStateChange -= OnStateChange;
    }

    private void OnStateChange(){
    //     switch(agent.ActiveState){
    //         case PursueSubState _:
    //             PursueUI.gameObject.SetActive(true);
    //             WanderUI.gameObject.SetActive(false);
    //             PathingUI.gameObject.SetActive(false);
    //             break;
    //         case FleeSubState _:
    //             PursueUI.gameObject.SetActive(false);
    //             WanderUI.gameObject.SetActive(false);
    //             PathingUI.gameObject.SetActive(false);
    //             break;
    //         case WanderSubState _:
    //             PursueUI.gameObject.SetActive(false);
    //             WanderUI.gameObject.SetActive(true);
    //             PathingUI.gameObject.SetActive(false);
    //             break;
    //         case FollowPathSubState:
    //             PursueUI.gameObject.SetActive(false);
    //             WanderUI.gameObject.SetActive(false);
    //             PathingUI.gameObject.SetActive(true);
    //             break;
    //         default:
    //             break;

    //     }
    }
}
