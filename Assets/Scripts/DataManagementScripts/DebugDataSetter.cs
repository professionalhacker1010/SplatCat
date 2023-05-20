using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class DebugDataSetter : MonoBehaviour
{
    [SerializeField] int level;
    [SerializeField] SceneReference baseGame;

    private void Start()
    {
        if (!Application.isPlaying)
        {
#if UNITY_EDITOR
            if (!SceneManager.GetSceneByPath(baseGame.ScenePath).isLoaded)
            {
                EditorSceneManager.OpenScene(baseGame.ScenePath, OpenSceneMode.Additive);
            }
#endif
        }
        else
        {

#if UNITY_EDITOR
            LevelManager.currLevel = level;
#endif
        }
    }
}
