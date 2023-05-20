using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paintbrush : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float paintbrushSpeed;
    [SerializeField] private float paintbrushRotation;

    private Vector3 offsetRight = new Vector3(-0.15f, 0f, 0f);
    private Vector3 offsetLeft = new Vector3(0.15f, 0f, 0f);
    private Vector3 throwOffsetRight = new Vector3(0.25f, 0f, 0f);
    private Vector3 throwOffsetLeft = new Vector3(-0.25f, 0f, 0f);

    public enum BrushState {inHand, thrown, onGround};
    public BrushState brushState;

    [SerializeField] private CircleCollider2D circleColl1;
    [SerializeField] private CircleCollider2D circleColl2;
    [SerializeField] private Sprite brushSprite, brushColorSprite, thrownSprite, thrownColorSprite;

    [SerializeField] private BoxCollider2D triggerCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer colorSpriteRenderer;
    [SerializeField] private GameObject player;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //player = GameObject.Find("Player");
        Physics2D.IgnoreCollision(circleColl1, player.GetComponent<Collider2D>()); //paintbrush ignores non-trigger collision with player
        Physics2D.IgnoreCollision(circleColl2, player.GetComponent<Collider2D>());

        brushState = BrushState.onGround;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.centerOfMass = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //update brush state
        if (brushState == BrushState.inHand)
        {
            rb.gravityScale = 0f;
            if (GameObject.Find("Player").GetComponent<CharacterControls>().m_FacingRight)
            {
                transform.position = player.transform.position + offsetRight;
                spriteRenderer.flipX = false;
                colorSpriteRenderer.flipX = false;
                transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.position = player.transform.position + offsetLeft;
                spriteRenderer.flipX = true;
                colorSpriteRenderer.flipX = true;
                transform.rotation = Quaternion.identity;
            }
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        else if (brushState == BrushState.thrown)
        {
            rb.gravityScale = 0f;
        }
        else
        {
            rb.gravityScale = 3f;
        }

        //initial throw brush
        if (InputSettings.Throw() && brushState == BrushState.inHand && LevelManager.currLevel > 2)
        {
            StartCoroutine(IgnoreBrush()); //disable trigger collider for a little bit so player doesn't immediately recatch brush
            ChangeBrushState(BrushState.thrown);
            if (player.GetComponent<CharacterControls>().m_FacingRight)
            {
                transform.position = transform.position + throwOffsetRight; //move out of player's collider
                rb.velocity = new Vector2(paintbrushSpeed, 0f);
                rb.angularVelocity = paintbrushRotation;
            }
            else
            {
                transform.position = transform.position + throwOffsetLeft; //move out of player's collider
                rb.velocity = new Vector2(-paintbrushSpeed, 0f);
                rb.angularVelocity = -paintbrushRotation;
            }
            
            AudioManager.Instance.PlaySFX("Throw_SFX");
            //circleColl1.isTrigger = false; // have collision when not in hand
            //circleColl2.isTrigger = false;
            player.GetComponent<Animator>().SetTrigger("throw");
        }
    }

    public void ChangeBrushState(BrushState state)
    {
        brushState = state;
        switch (state)
        {
            case BrushState.thrown:
                spriteRenderer.sprite = thrownSprite;
                colorSpriteRenderer.sprite = thrownColorSprite;
                Debug.Log("thrown");
                break;
            case BrushState.onGround:
                Debug.Log("onGround");
                triggerCollider.enabled = true;
                spriteRenderer.sprite = brushSprite;
                colorSpriteRenderer.sprite = brushColorSprite;
                rb.velocity = Vector2.zero;
                rb.gravityScale = 1f;
                rb.angularVelocity /= 2f;
                break;
            case BrushState.inHand:
                Debug.Log("inHand");
                triggerCollider.enabled = false;
                spriteRenderer.sprite = brushSprite;
                colorSpriteRenderer.sprite = brushColorSprite;
                break;
            default:
                break;
        }
    }

    IEnumerator IgnoreBrush()
    {
        triggerCollider.enabled = false;
        yield return new WaitForSeconds(0.22f);
        triggerCollider.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (brushState == BrushState.thrown)
        {
            if (col.gameObject.tag == "StaticPlatform" || col.gameObject.tag == "DrawnPlatform")
            {
                ChangeBrushState(BrushState.onGround);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            ChangeBrushState(BrushState.inHand);
            AudioManager.Instance.PlaySFX("BrushPickup_SFX");
            //circleColl1.isTrigger = true; // prevent collision with play while in hand
            //circleColl2.isTrigger = true;
        }
    }
}
