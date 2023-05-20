using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] GameObject MenuCanvas;
    [SerializeField] GameObject SettingsCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackButton()
    {
        AudioManager.Instance.PlaySFX("MenuClick2_SFX");
        MenuCanvas.SetActive(true);
        SettingsCanvas.SetActive(false);
    }
}
