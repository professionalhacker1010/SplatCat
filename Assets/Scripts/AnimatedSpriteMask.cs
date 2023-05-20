using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSpriteMask : MonoBehaviour
{
    [SerializeField] SpriteMask mask;
    [SerializeField] SpriteRenderer sr;

    void LateUpdate()
    {
        if (mask.sprite != sr.sprite)
        {
            mask.sprite = sr.sprite;
        }
    }
}
