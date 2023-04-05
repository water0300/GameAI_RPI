using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalInfoUI : MonoBehaviour
{
    private Animal _animal;
    public Slider hungerSlider;
    public Slider thirstSlider;
    public Slider matingSlider;
    private Transform _cameraTransform;
    void Start() {
        _cameraTransform = Camera.main.transform;
        _animal = GetComponentInParent<Animal>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = _cameraTransform.rotation;

        hungerSlider.value = _animal.CurrentHunger / _animal.maxHunger;
        thirstSlider.value = _animal.CurrentThirst / _animal.maxThirst;
    }
}
