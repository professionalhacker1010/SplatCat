using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossPattern : MonoBehaviour
{
    //internal variables
    GameObject starCurrent;
    List<GameObject> instantiatedObjects = new List<GameObject>(); //clear list between each phase, destroy each item;
    int phase = 1;
    float attackYRange, attackXRange;

    //prefabs
    [SerializeField] GameObject starPrefab, holePrefab, platformPrefab, smallEnemyPrefab, mediumEnemyPrefab, largeEnemyPrefab, shadow, deathScreen;

    [SerializeField] float tearPlatformScalePerFrame;

    private const string platformTooltip = "x,y=start position, z=direction (degrees counter-clockwise, starting from 6 o' clock), w=length";
    private const string enemyTooltip = "x,y=start position, z=type (0=small, 1=med, 2=big)";

    //inspector-defined patterns
    #region
    [SerializeField] List<Vector2> playerRespawnPoints;
    [Header("Phase 1")]
    [SerializeField] Vector2 p1StarLocation;
    [SerializeField] float p1TimeBetweenPatterns;
    [Tooltip(platformTooltip)]
    [SerializeField] Vector4[] p1PlatformPattern;
    [SerializeField] float p1TimeBetweenPlatforms;
    [Tooltip(enemyTooltip)]
    [SerializeField] Collider2D p1EnemyTrigger;
    [SerializeField] Vector4[] p1EnemyPattern;
    [SerializeField] float p1TimeBetweenEnemies;
    [SerializeField] Collider2D p1HolesTrigger;
    [SerializeField] float p1TimeBetweenHoles;

    [Header("Phase 2")]
    [SerializeField] Vector2 p2StarLocation;
    [SerializeField] float p2TimeBetweenPatterns;
    [Tooltip(platformTooltip)]
    [SerializeField] Vector4[] p2PlatformPattern;
    [SerializeField] float p2TimeBetweenPlatforms;
    [Tooltip(enemyTooltip)]
    [SerializeField] Collider2D p2EnemyTrigger;
    [SerializeField] Vector4[] p2EnemyPattern;
    [SerializeField] float p2TimeBetweenEnemies;
    [SerializeField] Collider2D p2HolesTrigger;
    [SerializeField] float p2TimeBetweenHoles;

    [Header("Phase 3")]
    [SerializeField] Vector2 p3StarLocation;
    [SerializeField] float p3TimeBetweenPatterns;
    [Tooltip(platformTooltip)]
    [SerializeField] Vector4[] p3PlatformPattern1;
    [SerializeField] Vector4[] p3PlatformPattern2;
    [SerializeField] float p3TimeBetweenPlatforms;
    [Tooltip(enemyTooltip)]
    [SerializeField] Vector4[] p3EnemyPattern1;
    [SerializeField] Vector4[] p3EnemyPattern2;
    [SerializeField] float p3TimeBetweenEnemies;
    [SerializeField] Collider2D p3Pattern2Trigger;
    [SerializeField] Collider2D p3HolesTrigger;
    [SerializeField] float p3TimeBetweenHoles;
    #endregion

    //external references
    GameObject player, brush;
    BoxCollider2D playerCollider;
    BossDamage bossDamage;

    //events
    public VecFloatIntEvent ChangeBossPosEvent;
    public UnityEvent ChangePhaseEvent;
    public UnityEvent EndPatternEvent;
    public UnityEvent StartPatternEvent;

    [System.Serializable]
    public class VecFloatIntEvent : UnityEvent<Vector2, float, int>
    {
        
    }

    void Start()
    {
        bossDamage = GetComponent<BossDamage>();
        bossDamage.OnPhaseReset += OnPhaseReset;

        //parameters: Vector2 = position, float = time, int = type of attack
        if (ChangeBossPosEvent == null)
            ChangeBossPosEvent = new VecFloatIntEvent();

        if (ChangePhaseEvent == null)
            ChangePhaseEvent = new UnityEvent();

        if (EndPatternEvent == null)
            EndPatternEvent = new UnityEvent();

        if (StartPatternEvent == null)
            StartPatternEvent = new UnityEvent();

        ChangePhaseEvent.AddListener(EndPhase);

        Vector2 worldDimensions = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight));
        attackXRange = worldDimensions.x;
        attackYRange = worldDimensions.y;
        player = GameObject.FindGameObjectWithTag("Player");
        brush = GameObject.Find("Paintbrush");
        playerCollider = player.GetComponent<BoxCollider2D>();

        ChangePhase(phase);
    }

    // Update is called once per frame
    void Update()
    {
        //check if player in range of the star
        if (starCurrent) {
            if (playerCollider.IsTouching(starCurrent.GetComponent<CircleCollider2D>())){
                ChangePhaseEvent.Invoke();
            }
        }

        if (phase == 1)
        {
            if (playerCollider.IsTouching(p1EnemyTrigger))
            {
                p1EnemyTrigger.gameObject.SetActive(false);
            }
           else if (playerCollider.IsTouching(p1HolesTrigger))
            {
                p1HolesTrigger.gameObject.SetActive(false);
            }
        }
        else if (phase == 2)
        {
            /*if (playerCollider.IsTouching(p2EnemyTrigger))
            {
                p2EnemyTrigger.gameObject.SetActive(false);
            }*/
            if (playerCollider.IsTouching(p2HolesTrigger))
            {
                p2HolesTrigger.gameObject.SetActive(false);
            }
        }
        else if (phase == 3)
        {
            if (playerCollider.IsTouching(p3HolesTrigger))
            {
                p3HolesTrigger.gameObject.SetActive(false);
            }
            else if (playerCollider.IsTouching(p3Pattern2Trigger))
            {
                p3Pattern2Trigger.gameObject.SetActive(false);
            }
        }
    }

    public void StopCoroutines()
    {
        StopAllCoroutines();
    }

    void OnPhaseReset()
    {
        StopCoroutines();
        EndPhase();
        ChangePhase(phase);
        ResetPlayer(phase);
    }

    public void EndAndChangePhase()
    {
        StopAllCoroutines();
        StartCoroutine(ChangePhaseRoutine());
    }

    IEnumerator ChangePhaseRoutine()
    {
        CharacterControls controls = playerCollider.gameObject.GetComponent<CharacterControls>();

        Destroy(starCurrent);
        starCurrent = null;
        AudioManager.Instance.PlaySFX("Scribble_SFX");
        yield return new WaitForEndOfFrame();

        StartCoroutine(controls.LockControls(2.3f));
        GameObject deathScreenClone = Instantiate(deathScreen, Vector3.zero, Quaternion.identity);
        yield return new WaitForSecondsRealtime(0.8f);

        EndPhase();

        AudioManager.Instance.PlaySFX("Scribble_SFX");
        ObjectManager.Instance.OnResetLevel();

        yield return new WaitForSecondsRealtime(0.7f);
        Destroy(deathScreenClone);

        ChangePhase(); 
    }

    public void EndPhase()
    {
        if (starCurrent != null) Destroy(starCurrent);
        starCurrent = null;
        foreach (GameObject obj in instantiatedObjects)
        {
            Destroy(obj);
        }
        instantiatedObjects.Clear();
        ObjectManager.Instance.DestroyDrawnPlatforms();
    }

    //change phase with auto increment
    void ChangePhase()
    {
        phase++;
        ChangePhase(phase);
    }

    //change phase specify phase
    public void ChangePhase(int phase)
    {
        if (phase < 4)
        {
            player.transform.position = playerRespawnPoints[phase - 1];
            brush.transform.position = playerRespawnPoints[phase - 1];
        }

        if (phase == 1)
        {
            p1EnemyTrigger.gameObject.SetActive(true);
            p1HolesTrigger.gameObject.SetActive(true);
            StartCoroutine(p1());
        }
        else if (phase == 2)
        {
            //p2EnemyTrigger.gameObject.SetActive(true);
            p2HolesTrigger.gameObject.SetActive(true);
            StartCoroutine(p2());
        }
        else if (phase == 3)
        {
            p3Pattern2Trigger.gameObject.SetActive(true);
            p3HolesTrigger.gameObject.SetActive(true);
            StartCoroutine(p3());
        }
        else if (phase == 4)
        {
            //end bossfight
            LevelManager.Instance.ChangeLevel(LevelManager.currLevel + 1);
        }
    }

    public void ResetPlayer(int phase)
    {
        player.transform.position = playerRespawnPoints[phase - 1];
        brush.transform.position = playerRespawnPoints[phase - 1];
        player.GetComponent<CharacterControls>().ChangeCharacterState(CharacterState.Grounded);
    }

    //coroutines for attack phases -------------------------------------------------------------------------
    #region
    IEnumerator p1()
    {
        StartPatternEvent.Invoke();
        starCurrent = Instantiate(starPrefab, p1StarLocation, Quaternion.identity);
        StartCoroutine(player.GetComponent<CharacterControls>().LockControls(0.5f));

        //tear platforms pattern
        yield return TearPlatformsDestination(p1TimeBetweenPlatforms, p1PlatformPattern);

       
        yield return new WaitUntil(() => !p1EnemyTrigger.gameObject.activeSelf);
        yield return new WaitForSeconds(p1TimeBetweenPatterns);

        //create enemies pattern
        yield return CreateEnemiesDestination(p1TimeBetweenEnemies, p1EnemyPattern);

        yield return new WaitUntil(() => !p1HolesTrigger.gameObject.activeSelf);

        //keep stabbing holes until player wins or dies
        while (true)
        {
            yield return StabHolesDestination(1, p1TimeBetweenHoles, new Vector2[] { player.transform.position });
        }
    }

    IEnumerator p2()
    {
        StartPatternEvent.Invoke();
        starCurrent = Instantiate(starPrefab, p2StarLocation, Quaternion.identity);
        StartCoroutine(player.GetComponent<CharacterControls>().LockControls(0.5f));
        yield return new WaitForEndOfFrame();
        //yield return MoveHandOnly(new Vector2(p2PlatformPattern[0].x, p2PlatformPattern[0].y), p2TimeBetweenPatterns);

        //tear platforms pattern
        yield return TearPlatformsDestination(p2TimeBetweenPlatforms, p2PlatformPattern);

        //yield return new WaitUntil(() => !p2EnemyTrigger.gameObject.activeSelf);
        yield return new WaitForSeconds(p2TimeBetweenPatterns);

        //create enemies pattern
        yield return CreateEnemiesDestination(p2TimeBetweenEnemies, p2EnemyPattern);

        yield return new WaitUntil (() => !p2HolesTrigger.gameObject.activeSelf);

        //keep stabbing holes until player wins or dies
        while (true)
        {
            yield return StabHolesDestination(1, p2TimeBetweenHoles, new Vector2[] { player.transform.position });
        }
    }

    IEnumerator p3()
    {
        StartPatternEvent.Invoke();
        starCurrent = Instantiate(starPrefab, p3StarLocation, Quaternion.identity);
        yield return new WaitForEndOfFrame();
        yield return MoveHandOnly(new Vector2(p3PlatformPattern1[0].x, p3PlatformPattern1[0].y), p3TimeBetweenPatterns);

        //tear platforms pattern1 
        yield return TearPlatformsDestination(p3TimeBetweenPlatforms, p3PlatformPattern1);

        yield return new WaitForSeconds(p3TimeBetweenPatterns);

        //create enemies pattern1
        yield return CreateEnemiesDestination(p3TimeBetweenEnemies, p3EnemyPattern1);

        yield return new WaitUntil(() => !p3Pattern2Trigger.gameObject.activeSelf);
        //while (p3Pattern2Trigger) yield return StabHolesDestination(1, p3TimeBetweenHoles, new Vector2[] { player.transform.position });

        yield return new WaitForSeconds(p3TimeBetweenPatterns);

        //tear platforms pattern2
        yield return TearPlatformsDestination(p3TimeBetweenPlatforms, p3PlatformPattern2);

        yield return new WaitForSeconds(p3TimeBetweenPatterns);

        //create enemies pattern2
        yield return CreateEnemiesDestination(p3TimeBetweenEnemies, p3EnemyPattern2);

        yield return MoveHandOnly(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 6f), 1f);
        yield return new WaitUntil(() => !p3HolesTrigger.gameObject.activeSelf);

        //keep stabbing holes until player wins or dies
        while (true)
        {
            yield return StabHolesDestination(1, p3TimeBetweenHoles, new Vector2[] { player.transform.position });
        }
    }
    #endregion

    //coroutines for attack patterns -------------------------------------------------------------------------
    #region
    IEnumerator MoveHandOnly(Vector2 newPos, float time)
    {
        ChangeBossPosEvent.Invoke(newPos, time, 1);
        yield return new WaitForSeconds(time);
    }

    IEnumerator StabHolesDestination(int holes, float timeBetween, Vector2[] destinations)
    {
        for (int i = 0; i < holes; i++)
        {
            ChangeBossPosEvent.Invoke(destinations[i], timeBetween, 0);
            yield return new WaitForSeconds(timeBetween);
            AudioManager.Instance.PlaySFX("Stabbing_SFX", 2);
            instantiatedObjects.Add(Instantiate(holePrefab, destinations[i], Quaternion.identity));
        }
    }

    //x,y = start position, z = direction, w = length
    IEnumerator TearPlatformsDestination(float timeBetween, Vector4[] destinations)
    {
        shadow.GetComponent<SpriteRenderer>().enabled = false;
        Vector2 newPos;
        for (int i = 0; i < destinations.Length; i++)
        {
            newPos = new Vector2(destinations[i].x, destinations[i].y);
            if (timeBetween != 0f)
            {
                ChangeBossPosEvent.Invoke(newPos, timeBetween, 1);
            }
            yield return new WaitForSeconds(timeBetween);
            AudioManager.Instance.PlaySFX("Tearing_SFX");

            //create object
            GameObject newPlatform = Instantiate(platformPrefab, newPos, Quaternion.identity);
            instantiatedObjects.Add(newPlatform);

            //determine end position based on input rotation and scale
            Vector2 endPlatformPos = Vector2.zero;
            newPlatform.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, destinations[i].z);
            float lengthY = platformPrefab.GetComponentInChildren<SpriteRenderer>().bounds.size.y * destinations[i].w * -1f;
            float radians = destinations[i].z * Mathf.Deg2Rad;
            endPlatformPos.x = newPos.x - Mathf.Sin(radians) * lengthY;
            endPlatformPos.y = newPos.y + Mathf.Cos(radians) * lengthY;

            //draw platform
            ChangeBossPosEvent.Invoke(endPlatformPos, (destinations[i].w / tearPlatformScalePerFrame) / 30f, 1);
            Vector3 newScale = new Vector3(newPlatform.transform.localScale.x, 0.0f, 1.0f);
            while (newScale.y <= destinations[i].w) {
                newScale.y += tearPlatformScalePerFrame;
                newPlatform.transform.localScale = newScale;
                yield return new WaitForSeconds(0.033f); //assuming 30fps
            } 
        }
        shadow.GetComponent<SpriteRenderer>().enabled = true;
        EndPatternEvent.Invoke();
    }

    //x,y=start position, z=type (0=small, 1=med, 2=big)
    IEnumerator CreateEnemiesDestination( float timeBetween, Vector4[] destinations)
    {
        for (int i = 0; i < destinations.Length; i++)
        {
            //move boss
            if (destinations[i].z == 2f)
            {
                ChangeBossPosEvent.Invoke(new Vector2(destinations[i].x, destinations[i].y + destinations[i].w), timeBetween, 0);
            }
            else
            {
                ChangeBossPosEvent.Invoke(destinations[i], timeBetween, 0);
            }

            yield return new WaitForSeconds(timeBetween);

            //create enemy
            if (destinations[i].z == 0f) {
                instantiatedObjects.Add(Instantiate(smallEnemyPrefab, destinations[i], Quaternion.identity));
            }
            else if (destinations[i].z == 1f)
            {
                instantiatedObjects.Add(Instantiate(mediumEnemyPrefab, destinations[i], Quaternion.identity));
            }
            else if (destinations[i].z == 1.5f)
            {
                GameObject enemy = Instantiate(mediumEnemyPrefab, destinations[i], Quaternion.identity);
                enemy.GetComponent<EnemyLinearPatrol>().mIsHorizontal = false;
                instantiatedObjects.Add(enemy);
            }
            else if (destinations[i].z == 2f)
            {
                GameObject enemy = Instantiate(largeEnemyPrefab, destinations[i], Quaternion.identity);
                enemy.GetComponent<EnemyCirclePatrol>().radius = destinations[i].w;
                enemy.GetComponent<EnemyCirclePatrol>().mSpeed = 2.5f;
                instantiatedObjects.Add(enemy);
            }
            AudioManager.Instance.PlaySFX("Stabbing_SFX", 2);
        }

        EndPatternEvent.Invoke();
    }
    #endregion
}
