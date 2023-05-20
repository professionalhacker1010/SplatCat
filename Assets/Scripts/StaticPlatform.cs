using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticPlatform : MonoBehaviour
{
    private void Start()
    {
        ObjectManager.Instance.AddPlatform(gameObject);
    }

    private void OnDestroy()
    {
        if (ObjectManager.Instance != null) ObjectManager.Instance.RemovePlatform(gameObject);
    }
}
