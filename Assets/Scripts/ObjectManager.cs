using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectManager : MonoBehaviour
{
    //singleton declaration
    #region
    private static ObjectManager _instance;
    public static ObjectManager Instance
    {
        get
        {
            //if (_instance == null) Debug.Log("The ObjectManager is NULL");

            return _instance;
        }
    }
    #endregion
    [SerializeField] PortfolioCamera portfolioCamera;   

    [SerializeField] int maxUnbakedDrips = 200;
    [SerializeField] Transform dripFolder;
   
    List<GameObject> platforms = new List<GameObject>();
    List<GameObject> enemies = new List<GameObject>();
    List<GameObject> drawnPlatforms = new List<GameObject>();
    List<Paint> drips = new List<Paint>();
    List<Paint> oldDrips = new List<Paint>();
    List<Paint> bakedDrips = new List<Paint>();
    bool bakingDrips = false;
    bool movingOldDrips = false;

    List<SpriteRenderer> overBackgrounds = new List<SpriteRenderer>();
    Sprite currBG, onlyOldDripsBG;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        LevelManager.Instance.OnLevelTransition += OnTransitionLevel;
        LevelManager.Instance.OnLevelReset += OnResetLevel;
    }

    private void Update()
    {
        if (drips.Count >= maxUnbakedDrips && !bakingDrips && !movingOldDrips)
        {
            StartCoroutine(BakeDrips());
        }
    }


    public void OnResetLevel()
    {
        MoveOldDrips();
        GetEnemies().Clear();
    }

    public void OnTransitionLevel()
    {
        StopAllCoroutines();
        bakingDrips = false;
        drips.Clear();
        oldDrips.Clear();
        bakedDrips.Clear();

        for (int i = dripFolder.childCount - 1; i >= 0; i--)
        {
            Destroy(dripFolder.GetChild(i).gameObject);
        }

        GetEnemies().Clear();

        currBG = null;
        onlyOldDripsBG = null;
    }

    #region drips
    IEnumerator BakeDrips()
    {
        Debug.Log("Baking drips");
        bakingDrips = true;
        
        bakedDrips = new List<Paint>(drips);
        oldDrips.AddRange(drips);

        drips.Clear();

        //wait until all drips stopped moving
        foreach (var drip in bakedDrips)
        {
            if (!drip.GetIsStopped()) 
                yield return new WaitUntil(() => drip.GetIsStopped());
        }

        yield return new WaitForEndOfFrame();

        //screenshot
        int width = (int)overBackgrounds[0].sprite.rect.width;
        int height = (int)overBackgrounds[0].sprite.rect.height;

        if (movingOldDrips)
        {
            foreach (var sr in overBackgrounds) sr.sprite = onlyOldDripsBG;
        }

        yield return new WaitForEndOfFrame();

        currBG = portfolioCamera.ScreenShotSprite(new Vector2(0.5f, 0.5f), width, height);


        yield return new WaitForEndOfFrame();

        //bake into a sprite
        if (movingOldDrips)
        {
            OverwriteSprite(onlyOldDripsBG, currBG);
        }
        
        foreach (var sr in overBackgrounds) sr.sprite = currBG;

        //clear drips
        foreach (var drip in bakedDrips)
        {
            drip.gameObject.SetActive(false);
        }
        bakingDrips = false;
    }

    public void MoveOldDrips()
    {
        StartCoroutine(MoveOldDripsRoutine());
    }

    IEnumerator MoveOldDripsRoutine()
    {
        yield return new WaitUntil(() => bakingDrips == false);

        movingOldDrips = true;

        oldDrips.AddRange(drips);
        drips.Clear();

        foreach (var drip in oldDrips)
        {
            drip.GetComponent<SpriteMask>().frontSortingOrder = 2;
            drip.gameObject.SetActive(true);
        }
        
        yield return BakeDrips();

        foreach (var drip in oldDrips)
        {
            Destroy(drip.gameObject);
        }
        oldDrips.Clear();
        movingOldDrips = false;
    }

    public void DestroyOldDrips()
    {
        foreach (var drip in oldDrips)
        {
            Destroy(drip);
        }
        oldDrips.Clear();
    }

    public void AddDrip(Paint drip)
    {
        drips.Add(drip);
        drip.transform.SetParent(dripFolder);
    }

    public void RemoveDrip(Paint drip)
    {
        drips.Remove(drip);
    }

    public void DestroyDrips()
    {
        foreach (var drip in drips){
            Destroy(drip);
        }
        drips.Clear();
    }
    #endregion

    #region static platforms
    public void AddPlatform(GameObject platform)
    {
        platforms.Add(platform);
    }

    public void RemovePlatform(GameObject platform)
    {
        platforms.Remove(platform);
    }

    public List<GameObject> GetPlatforms()
    {
        return platforms;
    }
    #endregion

    #region enemies
    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    public List<GameObject> GetEnemies()
    {
        return enemies;
    }
    #endregion

    #region drawn platforms
    public void AddDrawnPlatform(GameObject platform)
    {
        drawnPlatforms.Add(platform);
    }

    public void RemoveDrawnPlatform(GameObject platform)
    {
        drawnPlatforms.Remove(platform);
    }

    public List<GameObject> GetDrawnPlatforms()
    {
        return drawnPlatforms;
    }

    public void DestroyDrawnPlatforms()
    {
        foreach (var platform in drawnPlatforms)
        {
            Destroy(platform);
        }
        drawnPlatforms.Clear();
    }
    #endregion

    public void AddOverBackground(SpriteRenderer sr)
    {
        overBackgrounds.Add(sr);
        int width = (int)sr.sprite.rect.width;
        int height = (int)sr.sprite.rect.height;
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        portfolioCamera.AddCachedSprite(pivot, width, height);

        if (!onlyOldDripsBG)
        {
            //create cached sprite for old drips
            Rect rect = new Rect(0, 0, width, height);
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            onlyOldDripsBG = Sprite.Create(texture, rect, pivot);
            OverwriteSprite(onlyOldDripsBG, sr.sprite);
        }
        if (currBG) sr.sprite = currBG;
    }

    void OverwriteSprite(Sprite write, Sprite read)
    {
        write.texture.SetPixels(read.texture.GetPixels());
        write.texture.Apply();
    }

    public void RemoveOverBackground(SpriteRenderer sr)
    {
        overBackgrounds.Remove(sr);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
