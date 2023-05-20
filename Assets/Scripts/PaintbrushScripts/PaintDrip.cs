using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintDrip : Paint
{
    [SerializeField] List<Sprite> dripSprites;
    [SerializeField] float lerpDuration = 3; 
    float startValue = 0; 
    float endValue; 

    private void Awake()
    {
        GetComponent<SpriteMask>().sprite = dripSprites[Random.Range(0, dripSprites.Count - 1)];
    }
    protected override void Start()
    {
        base.Start();
        endValue = Random.Range(0.5f, 2f);
        StartCoroutine(DripDown(startValue, endValue, lerpDuration));
    }
}
