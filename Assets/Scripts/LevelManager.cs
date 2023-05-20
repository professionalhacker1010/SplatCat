using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class LevelManager : MonoBehaviour
{
    //singleton declaration
    #region
    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null) Debug.Log("The LevelManager is NULL");

            return _instance;
        }
    }
    #endregion

    [SerializeField] GameObject pagePrefab;
    [SerializeField] Camera PortfolioCamera;

    private List<GameObject> backgrounds;

    [SerializeField] public GameObject PauseMenu;
    [SerializeField] GameObject lineController;
    public bool paused;
    public static int currLevel = 1;

    public event Action OnLevelReset, OnLevelTransition;

    private void Awake()
    {
        backgrounds = new List<GameObject>();
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        paused = false;
        PauseMenu.SetActive(false);

#if !UNITY_EDITOR
        currLevel = SaveSystem.LoadData().level;
#endif
    }

    private void Start()
    {
        SceneManager.sceneLoaded += SetActiveScene;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F4))
        {
            ChangeLevel(currLevel + 1);
        }

        else if (Input.GetKeyDown(KeyCode.F3))
        {
            ChangeLevel(currLevel - 1);
        }
#endif

        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Menu")
        {
            paused = !paused;
            PauseMenu.SetActive(paused);
            lineController.SetActive(!paused);
        }
    }

    public void ResetLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(scene.name);
        SceneManager.LoadScene(scene.name, LoadSceneMode.Additive);

        if (OnLevelReset != null) OnLevelReset(); 
    }

    public void ChangeLevel(int level)
    {
        if (level == 1) lineController.SetActive(true);

        StartCoroutine(TransitionLevel(level));
        SaveSystem.SaveData(level);
    }

    //load next scene, play page flip animation, wait until page flip is done to unload other scene
    private IEnumerator TransitionLevel(int level)
    {
        Debug.Log("Transiiton to " + level);
        currLevel = level;

        Texture2D pageTexture = PortfolioCamera.GetComponent<PortfolioCamera>().ScreenShotTexture2D();

        yield return new WaitForEndOfFrame();

        if (!SceneManager.GetSceneByName("Menu").isLoaded && !SceneManager.GetSceneByName("Level8").isLoaded)
        {

            GameObject temp = Instantiate(pagePrefab, new Vector3(0f, 0f, 10f + currLevel), pagePrefab.transform.rotation);
            DontDestroyOnLoad(temp);
            SkinnedMeshRenderer[] portfolioMeshRenderer = temp.GetComponentsInChildren<SkinnedMeshRenderer>();
            Material portfolioMaterial = new Material(portfolioMeshRenderer[0].sharedMaterial);
            portfolioMaterial.SetTexture("_MainTex", pageTexture);
            portfolioMeshRenderer[0].sharedMaterial = portfolioMaterial;
           // portfolioMeshRenderer[1].sharedMaterial = portfolioMaterial;

            temp.transform.localScale = new Vector3(1.778f, 1.0f, 1.0f);

            if (currLevel <= 8)
                backgrounds.Add(temp);
        }

        if (SceneManager.GetSceneByName("Level8").isLoaded)
        {
            StartCoroutine(PortfolioScene());
        }

        //create page and apply texture
        GameObject page = Instantiate(pagePrefab, new Vector3(0f, 0f, 0.2f), pagePrefab.transform.rotation);
        DontDestroyOnLoad(page);

        SkinnedMeshRenderer[] meshRenderer = page.GetComponentsInChildren<SkinnedMeshRenderer>();
        meshRenderer[0].sharedMaterial.SetTexture("_MainTex", pageTexture);

        //play page flip transition
        FlipPage(page);

        if (OnLevelTransition != null) OnLevelTransition();

        //unload prev scene
        if (SceneManager.GetSceneByName("Menu").isLoaded) SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Menu"));
        else SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Level" + (level-1).ToString()));

        //load next scene
        string nextScene = "Level" + level.ToString();
        SceneManager.LoadScene(nextScene, LoadSceneMode.Additive);

        yield return new WaitForSeconds(1.0f);

        //audio
        string trackName = "Level" + level;
        AudioManager.Instance.PlayBGM(trackName);

        yield return new WaitForSeconds(1.0f);

        Destroy(page);
    }

    private void SetActiveScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        Scene activeScene = SceneManager.GetSceneByName("Level" + currLevel.ToString());
        if (activeScene != null && activeScene.isLoaded)
            SceneManager.SetActiveScene(activeScene);
    }

    private void FlipPage(GameObject page)
    {
        int flipsfx = UnityEngine.Random.Range(1, 3);
        string flipsfxstr = "SceneTransition" + flipsfx.ToString() + "_SFX";
        AudioManager.Instance.PlaySFX(flipsfxstr, 2);

        //play page flip transition
        page.transform.position = new Vector3(0f, 0f, -0.2f);
        page.transform.localScale = new Vector3(1.778f, 1.0f, 1.0f);
        foreach (var animator in page.GetComponentsInChildren<Animator>())
        {
            animator.SetTrigger("flip");
        }
    }

    private IEnumerator PortfolioPageFlip(GameObject page)
    {
        FlipPage(page);

        yield return new WaitForSeconds(1.0f);

        page.transform.position = new Vector3(0f, 20f, 0f);
    }

    private IEnumerator PortfolioScene()
    {
        Debug.Log("portfolio scene start");
        for (int i = 0; i<backgrounds.Count; i++)
        {
            yield return new WaitForSeconds(4.0f);
            StartCoroutine(PortfolioPageFlip(backgrounds[i]));
        }

        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Level" + currLevel.ToString()));
        SceneManager.LoadScene("Credits", LoadSceneMode.Additive);

        currLevel = 1;
        SaveSystem.SaveData(currLevel);

        yield return new WaitForSeconds(1.0f);

        foreach (var obj in backgrounds)
        {
            Destroy(obj);
        }
        backgrounds.Clear();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SetActiveScene;
    }
}