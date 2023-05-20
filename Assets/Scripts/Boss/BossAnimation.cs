using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimation : MonoBehaviour
{
    [SerializeField] GameObject shadow, hand;
    [Header("Shadow")]
    [SerializeField] float moveShadowTime;
    [SerializeField] float fadeInShadowTime;
    [SerializeField] Vector3 maxShadowScale;
    [SerializeField] float maxShadowOpacity;
    [Header("Hand")]
    [SerializeField] float handDownTime;
    [SerializeField] float handUpTime;

    SpriteRenderer shadowSpriteRenderer;
    Animator handAnimator;

    // Start is called before the first frame update
    void Awake()
    {
        shadowSpriteRenderer = shadow.GetComponent<SpriteRenderer>();
        handAnimator = hand.GetComponentInChildren<Animator>();
    }

    public void MoveHand(Vector2 pos, float time, int type)
    {
        StopAllCoroutines();
        StartCoroutine(MoveHandCoroutine(pos, time, type));
    }

    IEnumerator MoveHandCoroutine(Vector2 pos, float time, int type)
    {
        if (type == 0)
        {
            yield return MoveHandLifted(pos, time);
        }
        else if (type == 1)
        {
            yield return MoveHandOnPage(pos, time);
        }
    }

    IEnumerator MoveHandLifted(Vector2 pos, float time)
    {
        time -= 9 / 24f;
        float timer = 0.0f;
        float moveTime = Mathf.Min(time, moveShadowTime);
        float fadeTime = Mathf.Min(time, fadeInShadowTime);
        bool fadeInStarted = false;

        Vector3 startPosShadow = shadow.transform.position, startPosHand = hand.transform.position;
        handAnimator.SetTrigger("up");
        StartCoroutine(FadeShadow(moveTime, 0.0f, Vector3.zero));
        yield return new WaitForSeconds(3/24f);
        while (timer <= time)
        {
            //move
            if (timer <= moveTime)
            {
                shadow.transform.position = Vector3.Lerp(startPosShadow, pos, timer / moveTime);
                hand.transform.position = Vector3.Lerp(startPosHand, pos, timer / moveTime);
            }

            if (!fadeInStarted && timer > moveTime)
            {
                StartCoroutine(FadeShadow(time - moveTime, 1.0f, maxShadowScale));
                fadeInStarted = true;
            }

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        handAnimator.SetTrigger("down");
        
        yield return new WaitForSeconds(6/24f);
    }

    public IEnumerator MoveHandOnPage(Vector2 pos, float time)
    {
        float timer = 0.0f;
        Vector3 startPosShadow = shadow.transform.position, startPosHand = hand.transform.position;
        while (timer <= time)
        {
            //move shadow and hand to new spot on page
            shadow.transform.position = Vector3.Lerp(startPosShadow, pos, timer / time);
            hand.transform.position = Vector3.Lerp(startPosHand, pos, timer / time);

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
    }

    public void ChangePhaseAnimation()
    {
        StopAllCoroutines();
    }

    public void EndPatternAnimation()
    {
        handAnimator.SetTrigger("up");
        StartCoroutine(FadeShadow(fadeInShadowTime, 0.0f, Vector3.zero));
    }

    public void StartPatternAnimation()
    {
        handAnimator.SetTrigger("down");
        StartCoroutine(FadeShadow(fadeInShadowTime, 1.0f, maxShadowScale));
    }

    IEnumerator FadeShadow(float time, float alpha, Vector3 scale)
    {
        float timer = 0.0f;
        Vector3 startScale = shadow.transform.localScale;
        Color oldColor = shadowSpriteRenderer.color;
        while (timer <= time)
        {
            //fade shadow
            shadowSpriteRenderer.color = new Color(oldColor.r, oldColor.g, oldColor.b, Mathf.SmoothStep(oldColor.a , alpha, timer / time));

            //change scale of sadow
            shadow.transform.localScale = Vector3.Slerp(startScale, scale, timer / time);

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
    }
}
