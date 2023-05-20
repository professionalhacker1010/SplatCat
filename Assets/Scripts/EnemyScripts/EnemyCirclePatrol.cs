using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCirclePatrol : EnemyMovement
{
    public float radius = 1f;
 
    private Vector2 center;
    private float angle;
 
    private void Start()
    {
        center = transform.position;
    }

    public override void Move() 
    {
        angle += mSpeed * Time.deltaTime;

        var offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
        transform.position = center + offset;
    }
}
