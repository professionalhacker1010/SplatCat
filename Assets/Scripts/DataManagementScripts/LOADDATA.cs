using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LOADDATA : MonoBehaviour
{
    private string currScene;

    //DEBUG PURPOSES ONLY, DELETE LATER
    void Awake()
    {
        currScene = SceneManager.GetActiveScene().name;
        if (currScene != "Credits")
        {
            PersistentData.CurrLevel = SceneManager.GetActiveScene().name[5];
            PersistentData.CurrLevel -= 48;
        }

        if (!GameObject.Find("LevelManager")) //check if menu has been loaded
        {
            DontDestroyOnLoad(gameObject);
            StartCoroutine(LoadMenu());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator LoadMenu()
    {
        SceneManager.LoadScene("Menu");
        yield return new WaitForSeconds(0.1f);
        //PersistentData.DATALOADED = true;
        SceneManager.LoadScene(currScene);
    }
}
