using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarWinning : MonoBehaviour
{
    GameObject paintbrushObject;
    AudioSource audioSource;

    void OnTriggerEnter2D(Collider2D other)
    {
        paintbrushObject = GameObject.Find("Paintbrush");
        if (other.gameObject.name == "Player" && paintbrushObject.GetComponent<Paintbrush>().brushState == Paintbrush.BrushState.inHand)
        {
            other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
            other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            other.gameObject.GetComponent<CharacterControls>().enabled = false;
            AudioManager.Instance.PlaySFX("Winning_SFX", 1, AudioManager.Instance.starSource);

            //change scene to next level
            LevelManager.Instance.ChangeLevel(LevelManager.currLevel + 1);
        }
    }
}
