using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchSceneColor : MonoBehaviour
{
    public List<SpriteRenderer> sprites;
    public List<Material> materials;

    void Start()
    {
        ChangeColor(LevelManager.currLevel);
        SceneManager.activeSceneChanged += ChangeColorEvent;
    }

    void ChangeColorEvent(Scene one, Scene two)
    {
        if (two.name != "Menu" && two.name != "Credits")
        {
            ChangeColor((int)char.GetNumericValue(two.name[5]));
        }
    }

    void ChangeColor(int level)
    {
        
        Color color = Color.white;
        switch (level)
        {
            case 1:
                color = new Color(1.0f, 0.5886792f, 1.0f); //pink
                break;
            case 2:
                color = Color.yellow;
                break;
            case 3:
                color = new Color(1.0f, 0.6351262f, 0.0f); //orange
                break;
            case 4:
                color = new Color(0.8679245f, 0.289854f, 0.3713767f); //red
                break;
            case 5:
                color = new Color(0.6414292f, 0.4188679f, 1.0f); //purple
                break;
            case 6:
                color = new Color(0.002776734f, 0.735849f, 0.6991937f); //greenblue
                break;
            case 7:
                color = new Color(0.08198643f, 0.3732907f, 0.9245283f); //blue
                break;
            case 8:
                color = Color.grey;
                break;
            default:
                break;
        }

        foreach (SpriteRenderer sr in sprites)
        {
            if (!sr) continue;
            sr.color = color;
        }

        foreach (Material mat in materials)
        {
            if (!mat) continue;
            mat.SetColor("_Color", color);
        }
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= ChangeColorEvent;
    }
}
