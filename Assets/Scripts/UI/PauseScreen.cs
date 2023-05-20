using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] public GameObject menu; // Assign in inspector

    void Start()
    {
        if (!PersistentData.PauseShown) menu.SetActive(true);
        PersistentData.PauseShown = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputSettings.Menu())
        {
            menu.SetActive(!menu.activeInHierarchy);
            Debug.Log("pause");

        }
    }
}
