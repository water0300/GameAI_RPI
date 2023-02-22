using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour {
    
    private MapGenerator _mapGenerator;
    public TMP_Dropdown dropdown;
    private void Start() {
        _mapGenerator = FindObjectOfType<MapGenerator>();
    }

    public void OnMapGenerate(){
        // _mapGenerator.ClearMap();
        _mapGenerator.GenerateMapFromFile(dropdown.options[dropdown.value].text); //todo coupling UI text to logic a nono?
    }
    
    // public void OnMapDelete() => _mapGenerator.ClearMap();

    public void OnReset(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
