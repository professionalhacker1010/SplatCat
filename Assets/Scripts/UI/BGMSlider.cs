using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMSlider : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Slider BGMslider;

    public void Start()
    {
        //Adds a listener to the main slider and invokes a method when the value changes.
        BGMslider.onValueChanged.AddListener(delegate {ValueChangeCheckBGM(); });
    }

    //Invoked when a submit button is clicked.
    public void ValueChangeCheckBGM()
    {
        AudioManager.Instance.bgmSource.volume = BGMslider.value;
    }
}
