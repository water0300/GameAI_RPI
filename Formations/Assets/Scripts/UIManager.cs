using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void OnReset(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void On2Level(){
        SceneManager.LoadScene(0);
    }

    public void OnScalable(){
        SceneManager.LoadScene(1);
    }
}
