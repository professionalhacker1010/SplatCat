using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisions : MonoBehaviour
{
    [SerializeField] private GameObject splat;

    private void Start()
    {
        ObjectManager.Instance.AddEnemy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Player")
        {
            Debug.Log("eenmy");
            GameObject.Find("Game UI").GetComponent<GameUI>().Restart();
        }

        Paintbrush paintbrush = GameObject.Find("Paintbrush").GetComponent<Paintbrush>();
        if (collider.gameObject.name == "Paintbrush" &&
            paintbrush.brushState == Paintbrush.BrushState.thrown)
        {
            paintbrush.ChangeBrushState(Paintbrush.BrushState.onGround);

            //death effects
            Instantiate(splat, transform.position, transform.rotation);

            Destroy(this.gameObject);
            int deathsfx = Random.Range(1, 4);
            string deathsfxstr = "PaintSplatter" + deathsfx.ToString() + "_SFX";
            AudioManager.Instance.PlaySFX(deathsfxstr);
        }

        if (collider.gameObject.tag == "DrawnPlatform")
        {
            Destroy(collider.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (ObjectManager.Instance != null) ObjectManager.Instance.RemoveEnemy(gameObject);
    }
}
