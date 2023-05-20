using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AspectRatioCorrection : MonoBehaviour
{
    float aspectRatio;

    // Start is called before the first frame update
    void Start()
    {
        aspectRatio = Mathf.Round(Camera.main.aspect * 10) / 10f;

        CorrectAspectRatio(SceneManager.GetActiveScene(), SceneManager.GetActiveScene());

        SceneManager.activeSceneChanged += CorrectAspectRatio;
    }

    void CorrectAspectRatio(Scene current, Scene next)
    {
        switch (aspectRatio)
        {
            case 1.8f: //16:9 aspect
                Camera.main.orthographicSize = 5.0f;
                break;
            case 1.6f: //16:10 aspect
                Camera.main.orthographicSize = 5.56f;
                break;
            default:
                break;
        }
    }
}
