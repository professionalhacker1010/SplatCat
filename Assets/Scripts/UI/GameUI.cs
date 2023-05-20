using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [SerializeField] GameObject GUI;
    private GameObject Metrics;
    [SerializeField] GameObject deathScreen;

    // Start is called before the first frame update

    void Start()
    {
        UpdateGUI(SceneManager.GetActiveScene(), SceneManager.GetActiveScene());
        SceneManager.activeSceneChanged += UpdateGUI;
    }

    void UpdateGUI(Scene current, Scene next)
    {
        //set GUI active only when playing game (ie not the menu screen)
        if (next.name == "Menu" || next.name == "Credits" || next.name == "Level9" || next.name == "BaseGame")
        {
            GUI.SetActive(false);
        }
        else
        {
            GUI.SetActive(true);
        }
    }

    public void Restart()
    {
        Metrics = GameObject.Find("Metrics");


        switch (LevelManager.currLevel)
        {
            case 3:
                Metrics.GetComponent<Metrics>().d3++;
                break;
            case 4:
                Metrics.GetComponent<Metrics>().d4++;
                break;
            case 5:
                Metrics.GetComponent<Metrics>().d5++;
                break;
            case 6:
                Metrics.GetComponent<Metrics>().d6++;
                break;
            case 7:
                Metrics.GetComponent<Metrics>().d7++;
                break;
            case 8:
                Metrics.GetComponent<Metrics>().d8++;
                break;
            default:
                break;
        }

        if (LevelManager.currLevel == 8)
        {
            StartCoroutine(GameObject.FindGameObjectWithTag("Boss").GetComponent<BossDamage>().ResetPhase());
        }
        else
        {
            StartCoroutine(DeathScreen());
        }
    }

    IEnumerator DeathScreen()
    {
        AudioManager.Instance.PlaySFX("Scribble_SFX");
        StartCoroutine(GameObject.Find("Player").GetComponent<CharacterControls>().LockControls(2.3f));
        GameObject deathScreenClone = Instantiate(deathScreen, Vector3.zero, Quaternion.identity);
        DontDestroyOnLoad(deathScreenClone);
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(0.7f);

        Time.timeScale = 1;

        AudioManager.Instance.PlaySFX("Scribble_SFX");
        LevelManager.Instance.ResetLevel();

        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(0.8f);

        Time.timeScale = 1;
        Destroy(deathScreenClone);
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= UpdateGUI;
    }
}