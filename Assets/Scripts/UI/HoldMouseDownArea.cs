using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HoldMouseDownArea : MonoBehaviour
{
    [SerializeField] BoxCollider2D mouseDownArea;
    [SerializeField] Image fillImage;
    [SerializeField] float holdDownTime;

    bool mouseDown;
    Camera cam;

    public UnityEvent OnMouseHeldSuccess;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //mouse first pressed
        if (Input.GetMouseButtonDown(0) && mouseDownArea.OverlapPoint(cam.ScreenToWorldPoint(Input.mousePosition)))
        {
            mouseDown = true;
            StartCoroutine(ProcessMouseHeldDown());
        }

        //mouse up
        if (Input.GetMouseButtonUp(0) && mouseDown)
        {
            mouseDown = false;
            StopAllCoroutines();
            fillImage.fillAmount = 0f;
        }
    }

    IEnumerator ProcessMouseHeldDown()
    {
        float time = 0f;
        while (time < holdDownTime)
        {
            time += Time.deltaTime;
            float lerp = time / holdDownTime;
            fillImage.fillAmount = lerp;
            yield return new WaitForEndOfFrame();
        }

        fillImage.fillAmount = 1;
        OnMouseHeldSuccess.Invoke();
        AudioManager.Instance.PlaySFX("Scribble_SFX", 3);

        yield return new WaitForSeconds(1.0f);
        fillImage.fillAmount = 0;
    }
}
