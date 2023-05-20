using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverBackground : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var sr = GetComponent<SpriteRenderer>();
        var objectManager = ObjectManager.Instance;
        if (sr && objectManager)
        {
            objectManager.AddOverBackground(sr);
        }

        //correct position to middle of screen
        Camera cam = Camera.main;
        float y = cam.pixelHeight / 2.0f;
        float x = cam.pixelWidth / 2.0f;
        Vector3 newPos = cam.ScreenToWorldPoint(new Vector2(x, y));
        newPos.z = transform.position.z;
        transform.position = newPos;

    }

    private void OnDestroy()
    {
        var sr = GetComponent<SpriteRenderer>();
        var objectManager = ObjectManager.Instance;
        if (objectManager && sr)
        {
            objectManager.RemoveOverBackground(sr);
        }
    }
}
