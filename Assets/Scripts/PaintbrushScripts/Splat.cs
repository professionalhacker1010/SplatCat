using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splat : Paint
{
    [SerializeField] private float splatTime = 0.5f;
    [SerializeField] Vector3 maxSplatScale = new Vector3(0.6f, 0.6f, 1.0f);

    protected override void Start()
    {
        base.Start();
        transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0, 360));
        StartCoroutine(Splatter(splatTime, maxSplatScale));
    }
}
