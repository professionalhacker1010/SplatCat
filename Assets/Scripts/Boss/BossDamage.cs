using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BossDamage : MonoBehaviour
{
    [SerializeField] Collider2D bossCollider;
    [SerializeField] GameObject deathScreen;
    Collider2D playerCollider;

    public event Action OnPhaseReset;
    public static bool phaseResetting = false;

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
    }

    public void CheckDamagePlayer(Vector2 pos, float time, int type)
    {
        if (phaseResetting) return;
        if (type == 0)
        {
            StartCoroutine(CheckStabCollision(time));
        }
        else if (type == 1)
        {
            StartCoroutine(CheckTearCollision(time));
        }
    }

    IEnumerator CheckStabCollision(float time)
    {
        yield return new WaitForSeconds(time);
        if (playerCollider && bossCollider.IsTouching(playerCollider))
        {
            Debug.Log("stab");
            StartCoroutine(ResetPhase());
        }
    }

    IEnumerator CheckTearCollision(float time)
    {
        float timer = 0.0f;

        while (timer <= time)
        {
            if (playerCollider && bossCollider.IsTouching(playerCollider))
            {
                Debug.Log("tear");
                StartCoroutine(ResetPhase());
            }
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
    }

    public IEnumerator ResetPhase()
    {
        phaseResetting = true;
        AudioManager.Instance.PlaySFX("Scribble_SFX");
        yield return new WaitForEndOfFrame();
        StartCoroutine(GameObject.Find("Player").GetComponent<CharacterControls>().LockControls(2.3f));
        //Debug.Log("restart");
        GameObject deathScreenClone = Instantiate(deathScreen, Vector3.zero, Quaternion.identity);
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(0.7f);

        Time.timeScale = 1;

        AudioManager.Instance.PlaySFX("Scribble_SFX");
        ObjectManager.Instance.OnResetLevel();

        if (OnPhaseReset != null) OnPhaseReset();

        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(0.8f);

        Time.timeScale = 1;
        Destroy(deathScreenClone);
        phaseResetting = false;
    }
}
