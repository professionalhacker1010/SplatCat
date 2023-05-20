using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public enum CharacterState
{
	Grounded,
	Jump,
	Fall
}

public class CharacterControls : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] private float m_JumpForce;							// Amount of force added when the player jumps.
    [SerializeField] private float runSpeed;
	[SerializeField] private float maxFallSpeed;
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping
	[SerializeField] private float m_CoyoteTime;							// Max time after falling that player can still jump
	[SerializeField] private float m_JumpBufferTime;					// Max time before reaching ground that player input results in jump
	[SerializeField] private float m_CatchHeight;                        // Max height of gaps player gets lifted over
	[SerializeField] private float m_CatchHead;                         // Max distance where head collision is corrected to continue moving upwards
	[SerializeField] private PhysicsMaterial2D noFriction;
	[SerializeField] private PhysicsMaterial2D fullFriction;
	[SerializeField] private float m_GravityScaleVariableJump;			// shorten jump height by temporarily increasing gravity scale

	[Header("Spawn and Collision")]
	[SerializeField] private ContactFilter2D m_WhatIsPlatform;					// filter determining what is a platform
	[SerializeField] private Transform spawn;
	[SerializeField] private BoxCollider2D m_LeftCollider, m_RightCollider, m_BottomCollider, m_TopCollider; //boxes for collision detection

	//internal tracking variables
	[HideInInspector] public CharacterState m_CharacterState = CharacterState.Grounded;
	[HideInInspector] public bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private bool m_HasRightCollision = false, m_HasLeftCollision = false;
	private Vector3 m_Velocity = Vector3.zero;
	private Vector3 m_VelocityPrevFrame = Vector3.zero, m_NewVelocity = Vector3.zero;
	[HideInInspector] public float horizontalMove = 0f;          //horizontal input value
	float m_CoyoteTimer = 0f, m_JumpBufferTimer = 0f;
	Collision2D m_PlatformCollision = null;
	private float m_GravityScaleDefault;

	//component references
	private Rigidbody2D m_Rigidbody2D;
	private SpriteRenderer m_SpriteRenderer;
	private BoxCollider2D m_BoxCollider2D;
	private Animator m_animator;
	private AudioSource m_audioSource;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	private int walkingSFX = 0;
	private bool CR_RUNNING = false;

	private bool m_lockedControls = false;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
		m_BoxCollider2D = GetComponent<BoxCollider2D>();
		m_animator = GetComponent<Animator>();
		m_audioSource = gameObject.AddComponent<AudioSource>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

    private void Start()
    {
		transform.position = spawn.position;
		m_GravityScaleDefault = m_Rigidbody2D.gravityScale;
		if (LevelManager.currLevel == 5) Flip();
	}

    private void Update()
    {
		if (!m_lockedControls)
		{
			m_HasLeftCollision = false;
			m_HasRightCollision = false;
			if (m_Rigidbody2D.velocity.x > 0)
			{
				m_HasRightCollision = m_RightCollider.IsTouchingLayers(m_WhatIsPlatform.layerMask);
			}
			else if (m_Rigidbody2D.velocity.x < 0)
			{
				m_HasLeftCollision = m_LeftCollider.IsTouchingLayers(m_WhatIsPlatform.layerMask);
			}

			horizontalMove = Input.GetAxisRaw("Horizontal");

			switch (m_CharacterState)
			{
				case CharacterState.Grounded:
					UpdateGrounded();
					break;
				case CharacterState.Jump:
					UpdateJump();
					break;
				case CharacterState.Fall:
					UpdateFall();
					break;
				default:
					break;
			}

			//update animations
			m_animator.SetFloat("velocityX", Mathf.Abs(horizontalMove));

			// If the input is moving the player right and the player is facing left...
			if (horizontalMove > 0 && !m_FacingRight)
			{
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (horizontalMove < 0 && m_FacingRight)
			{
				Flip();
			}

			m_Rigidbody2D.velocity = m_NewVelocity;
			m_VelocityPrevFrame = m_Rigidbody2D.velocity;
		}
	}

	private void FixedUpdate()
	{

	}

    private void UpdateGrounded()
    {
		if (m_Rigidbody2D.velocity.y < 0.0f && !m_BottomCollider.IsTouchingLayers(m_WhatIsPlatform.layerMask) && !(m_HasLeftCollision || m_HasRightCollision))
        {
			m_CoyoteTimer -= Time.deltaTime;
			if (m_CoyoteTimer <= 0.0f)
			{
				ChangeCharacterState(CharacterState.Fall);
			}
        }
        else
        {
			m_CoyoteTimer = m_CoyoteTime;
        }

		Move();

		if (m_CoyoteTimer > 0.000f && InputSettings.Jump())
		{
			ChangeCharacterState(CharacterState.Jump);
		}

		//adjust friction based on whether player is moving or not
		if (m_BottomCollider.IsTouchingLayers(m_WhatIsPlatform.layerMask))
		{
			if (horizontalMove < 0.05f && horizontalMove > -0.05f)
            {
				m_Rigidbody2D.sharedMaterial = fullFriction;
				if (m_audioSource.isPlaying) m_audioSource.Stop();
            }
            else
            {
				m_Rigidbody2D.sharedMaterial = noFriction;
				

			}
		}

		if (horizontalMove != 0.0f && !CR_RUNNING)
		{
			StartCoroutine(FootstepSFX());
		}

		if (horizontalMove != 0.0f) { if (!m_audioSource.isPlaying) { m_audioSource.Play(); } }
	}

	IEnumerator FootstepSFX()
	{
		CR_RUNNING = true;
		string walkingStr = "Footstep";
		walkingStr += walkingSFX;
		walkingStr += "_SFX";
		//Debug.Log(walkingStr);
		AudioManager.Instance.PlaySFX(walkingStr);
		yield return new WaitForSeconds(0.4f);
		if (walkingSFX == 0) walkingSFX = 1;
		else walkingSFX = 0;
		CR_RUNNING = false;
	}

	private void UpdateJump()
    {
		//move player when head just barely collides with edge of platform
		List<Collider2D> cols = new List<Collider2D>();
		if (m_TopCollider.OverlapCollider(m_WhatIsPlatform, cols) != 0)
        {
			foreach (Collider2D otherCollider in cols)
            {
				float leftDistance = 0f, rightDistance = 0f;
				if (otherCollider.gameObject.tag == "DrawnPlatform")
				{
					EdgeCollider2D edgeCollider = ((EdgeCollider2D)otherCollider);
					leftDistance = edgeCollider.points[edgeCollider.points.Length - 1].x - m_BoxCollider2D.bounds.min.x;
					rightDistance = m_BoxCollider2D.bounds.max.x - edgeCollider.points[0].x;
				}
				else
				{
					leftDistance = otherCollider.bounds.max.x - m_BoxCollider2D.bounds.min.x;
					rightDistance = m_BoxCollider2D.bounds.max.x - otherCollider.bounds.min.x;
				}

				if (leftDistance > 0f && leftDistance <= m_CatchHead)
				{
					//Debug.Log("Catch head");
					transform.position = new Vector2(transform.position.x + 0.15f, transform.position.y);
					m_NewVelocity = m_VelocityPrevFrame;
					return;
				}
				else if (rightDistance > 0f && rightDistance <= m_CatchHead)
				{
					//Debug.Log("Catch head");
					transform.position = new Vector2(transform.position.x - 0.15f, transform.position.y);
					m_NewVelocity = m_VelocityPrevFrame;
					return;
				}
			}

		}

		if (m_AirControl)
		{
			Move();
		}

		if (m_NewVelocity.y < 0 || InputSettings.JumpRelease()) // shorter jump when releasing button earlier
		{
			m_Rigidbody2D.gravityScale = m_GravityScaleVariableJump;
			ChangeCharacterState(CharacterState.Fall);
        }
	}

    private void UpdateFall()
    {
		if (m_AirControl)
		{
			Move();
		}

		m_NewVelocity = Vector2.ClampMagnitude(m_NewVelocity, maxFallSpeed);

		if (InputSettings.Jump())
		{
			m_JumpBufferTimer = m_JumpBufferTime;
		}
		else
		{
			m_JumpBufferTimer -= Time.deltaTime;
		}

		if (m_BottomCollider.IsTouchingLayers(m_WhatIsPlatform.layerMask))
		{
			if (m_JumpBufferTimer > 0f)
			{
				m_Rigidbody2D.gravityScale = m_GravityScaleDefault;
				m_CoyoteTimer = m_CoyoteTime;
				ChangeCharacterState(CharacterState.Jump);
			}
			else
			{
				OnLandEvent.Invoke();
				ChangeCharacterState(CharacterState.Grounded);
			}
		}
	}

    //This code executes only when first changing to state
    public void ChangeCharacterState(CharacterState state)
    {
		if (state == m_CharacterState) return;

		m_CharacterState = state;

        switch (state) {

			case CharacterState.Grounded:
				//Debug.Log("Ground");
				m_Rigidbody2D.gravityScale = m_GravityScaleDefault;
				m_animator.SetTrigger("land");
				m_CoyoteTimer = m_CoyoteTime;
				break;

			case CharacterState.Jump:
				if (m_CoyoteTimer < m_CoyoteTime) Debug.Log("CoyoteJump");
				//else Debug.Log("Jump");
				m_JumpBufferTimer = 0f;
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));   // Add a vertical force to the player.
				m_animator.SetTrigger("jump");
				m_Rigidbody2D.sharedMaterial = noFriction;
				AudioManager.Instance.PlaySFX("Jump_SFX");
				break;

			case CharacterState.Fall:
				//Debug.Log("Fall");
				break;

			default:
				break;
		}
    }

	private void Move()
	{
		// Move the character by finding the target velocity
		Vector3 targetVelocity = new Vector2(horizontalMove * runSpeed * Time.fixedDeltaTime * 10f, m_Rigidbody2D.velocity.y);

		//get collision with sides
		List<Collider2D> cols = new List<Collider2D>();
		if (m_HasRightCollision) m_RightCollider.OverlapCollider(m_WhatIsPlatform, cols);
		else if (m_HasLeftCollision) m_LeftCollider.OverlapCollider(m_WhatIsPlatform, cols);

		bool zeroX = false;
		if (cols.Count != 0)
		{
			foreach (Collider2D otherCollider in cols)
            {
				//lift player when just barely colliding with edge of platform
				float distance = 0f;
				if (otherCollider.gameObject.tag == "DrawnPlatform" && m_PlatformCollision != null)
				{
					distance = ((EdgeCollider2D)otherCollider).ClosestPoint(transform.position).y - m_BoxCollider2D.bounds.min.y;
				}
				else
				{
					distance = otherCollider.bounds.max.y - m_BoxCollider2D.bounds.min.y;
				}

				//lift player
				if (distance <= m_CatchHeight && distance > 0f)
				{
					Debug.Log("Catch height");
					transform.position = new Vector2(transform.position.x, transform.position.y + 0.1f);
				}
				//or if colliding with wall or edge of ground, stop applying x velocity
				else
				{
					zeroX = true;
				}
			}

		}

		// And then smoothing it out and applying it to the character
		m_NewVelocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
		if (zeroX) m_NewVelocity.x = 0f;
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Flip sprite
		m_SpriteRenderer.flipX = !m_FacingRight;
	}

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "DrawnPlatform")
        {
			m_PlatformCollision = null;
        }
    }

	public IEnumerator LockControls(float time)
    {
		m_lockedControls = true;
		m_animator.SetFloat("velocityX", 0.0f);
		m_Rigidbody2D.velocity = new Vector2(0.0f, m_Rigidbody2D.velocity.y);
		yield return new WaitForSeconds(time);
		m_lockedControls = false;
    }
}