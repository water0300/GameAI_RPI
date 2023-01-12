using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject winScreen;
    public GameObject mainUI;
    public TMP_Text winText;
    public TMP_Text attemptText;
    public Transform playerSpawn;
    public GameObject playerPrefab;
    public CinemachineVirtualCamera vcam;

    private GameObject _currPlayer;

    private int _attempts = 0;

    void Start(){
        attemptText.text = $"Attempts: {_attempts}";
        PlayerInit();
    }

    public void FailRestart(){
        _attempts++;
        attemptText.text = $"Attempts: {_attempts}";
        _currPlayer.GetComponent<Player>().enabled = false;
        PlayerInit();
    }

    private void PlayerInit(){
        _currPlayer = Instantiate(playerPrefab, playerSpawn.position, playerSpawn.rotation);
        vcam.LookAt = _currPlayer.transform;
        vcam.Follow = _currPlayer.transform;
    }
 
    public void WinCon(){
        mainUI.SetActive(false);
        winScreen.SetActive(true);
        _attempts++;
        winText.text = $"Won in {_attempts} attempts!";
    }

    public void RestartScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
