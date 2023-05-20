using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject LevelManager;
    [SerializeField] private GameObject lineController;

    public void Resume()
    {
        LevelManager lm = LevelManager.GetComponent<LevelManager>();
        lm.paused = !lm.paused;
        lm.PauseMenu.SetActive(lm.paused);
        Time.timeScale = 1.0f;
        lineController.SetActive(!lm.paused);
    }

    public void SaveAndQuit()
    {
        Application.Quit();
    }
}
