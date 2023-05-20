using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject MenuCanvas;
    [SerializeField] GameObject SettingsCanvas;
    [SerializeField] SceneReference BaseGame;
    
    private void Awake()
    {
        if (!SceneManager.GetSceneByPath(BaseGame.ScenePath).isLoaded)
            SceneManager.LoadSceneAsync(BaseGame, LoadSceneMode.Additive);
    }

    public void Start()
    {
        SettingsCanvas.SetActive(false);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Menu"));
        AudioManager.Instance.PlayBGM("Menu");
    }

    public void OnSceneLoaded()
    {
        SettingsCanvas.SetActive(false);
    }

    public void StartGame()
    {
        AudioManager.Instance.PlaySFX("MenuClick1_SFX");
        AudioManager.Instance.bgmSource.Stop();
        LevelManager.Instance.ChangeLevel(LevelManager.currLevel);
    }

    public void ResetSave()
    {
        Debug.Log("reset save");
        LevelManager.currLevel = 1;
        SaveSystem.SaveData(LevelManager.currLevel);
    }

    public void GoToSettings()
    {
        SettingsCanvas.SetActive(true);
        MenuCanvas.SetActive(false);
        AudioManager.Instance.PlaySFX("MenuClick2_SFX");
    }

    public void QuitGame()
    {
        AudioManager.Instance.PlaySFX("MenuClick1_SFX");
        Application.Quit();
    }

    public void ToCredits()
    {
        AudioManager.Instance.PlaySFX("MenuClick1_SFX");
        SceneManager.UnloadSceneAsync("Menu");
        SceneManager.LoadSceneAsync("Credits", LoadSceneMode.Additive);
    }
}