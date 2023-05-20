using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float mSpeed;
    protected Collider2D mCollider;

    protected void Awake()
    {
        mCollider = GetComponent<Collider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected void Update()
    {
        Move();
    }

    public virtual void Move()
    {

    }
}
