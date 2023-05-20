using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paint : MonoBehaviour
{
    protected bool isStopped = false;
    public bool GetIsStopped() => isStopped;

    BossPattern bossPattern;
    BossDamage bossDamage;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        ObjectManager.Instance.AddDrip(this);
        LevelManager.Instance.OnLevelReset += OnResetLevel;

        bossDamage = GameObject.FindObjectOfType<BossDamage>();
        if (bossDamage)
        {
            bossDamage.OnPhaseReset += OnResetLevel;
        }

        bossPattern = GameObject.FindObjectOfType<BossPattern>();
        if (bossPattern)
        {
            bossPattern.ChangePhaseEvent.AddListener(OnResetLevel);
        }
    }

    void OnResetLevel()
    {
        if (this && gameObject)
        {
            StopAllCoroutines();
            isStopped = true;
        }

    }

    protected IEnumerator Splatter(float splatTime, Vector3 endScale)
    {
        float timer = 0;
        while (timer < splatTime)
        {
            transform.localScale = Vector3.Slerp(Vector3.forward, endScale, timer / splatTime);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
    }

    protected IEnumerator DripDown(float startValue, float endValue, float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float currValue = Mathf.Lerp(startValue, endValue, timeElapsed / duration);
            transform.localScale = new Vector3(transform.localScale.x, currValue, 0f);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        isStopped = true;
    }

    private void OnDestroy()
    {
        if (bossDamage) bossDamage.OnPhaseReset -= OnResetLevel;
        if (bossPattern) bossPattern.ChangePhaseEvent.RemoveListener(OnResetLevel);
        LevelManager.Instance.OnLevelReset -= OnResetLevel;
    }
}
