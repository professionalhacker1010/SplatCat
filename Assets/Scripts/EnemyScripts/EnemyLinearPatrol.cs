using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLinearPatrol : EnemyMovement
{
    //inherited data:
    //float mSpeed
    //BoxCollider2D mCollider

    public bool mIsHorizontal; //horizontal or vertical patrolling?
    float mDirection = 1.0f;
    bool mCheckCollision = true;

    public override void Move()
    {
        if (mCheckCollision)
        {
            //check collision with platforms and enemies
            foreach (var item in ObjectManager.Instance.GetPlatforms())
            {
                if (mCollider.IsTouching(item.GetComponent<BoxCollider2D>()))
                {
                    mDirection *= -1;
                    StartCoroutine(StaggerCollisionCheck());
                    return;
                }
            }

            foreach (var item in ObjectManager.Instance.GetEnemies())
            {
                if (item == gameObject) continue;
                if (mCollider.IsTouching(item.GetComponent<Collider2D>()))
                {
                    mDirection *= -1;
                    StartCoroutine(StaggerCollisionCheck());
                    return;
                }
            }
        }

        if (mIsHorizontal)
        {
            transform.position = new Vector2(transform.position.x + mSpeed * mDirection * Time.deltaTime, transform.position.y);
        }
        else
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + mSpeed * mDirection * Time.deltaTime);
        }


    }

    IEnumerator StaggerCollisionCheck()
    {
        mCheckCollision = false;
        yield return new WaitForSeconds(0.7f);
        mCheckCollision = true;
    }
}
