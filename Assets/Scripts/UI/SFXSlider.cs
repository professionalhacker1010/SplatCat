using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXSlider : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Slider SFXslider;

    public void Start()
    {
        //Adds a listener to the main slider and invokes a method when the value changes.
        SFXslider.onValueChanged.AddListener(delegate {ValueChangeCheckSFX(); });
    }

    //Invoked when a submit button is clicked.
    public void ValueChangeCheckSFX()
    {
        AudioManager.Instance.sfxSource.volume = SFXslider.value;
    }
}
