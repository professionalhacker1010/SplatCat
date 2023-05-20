using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    private void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Credits"));
    }

    public void BackToMenu()
    {
        AudioManager.Instance.PlaySFX("MenuClick1_SFX");
        SceneManager.UnloadSceneAsync("Credits");
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
    }
}
