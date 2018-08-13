using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// player movement mostly from Unity3D tutorial

public class PlayerController : MonoBehaviour
{
    public float maxFood = 100.0f;
    public float maxChitin = 100.0f;
    public float foodSubtraction = 0.5f;
    public float chitinSubtraction = 0.02f;
    public float algaeFood = 30.0f;
    public float shrimpFood = 20.0f;
    public float shrimpChitin = 10.0f;
    public float speedOfLiving = 1.0f;
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private float food;
    private float chitin;
    public Slider foodBar;
    public Slider chitinBar;
    public GameObject GameOverMenu;
    public GameObject GameWinMenu;
    public Button PauseButton;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public float minGroundNormalY = .65f;
    public float gravityModifier = 1f;

    private Vector2 targetVelocity;
    private bool grounded;
    private Vector2 groundNormal;
    private Rigidbody2D rb2d;
    private Vector2 velocity;
    private ContactFilter2D contactFilter;
    private RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    private List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);


    private const float minMoveDistance = 0.001f;
    private const float shellRadius = 0.01f;
    private const int depthForDeath = -40;

    public AudioSource GameOverSound;
    public AudioSource GameWinSound;


    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {

        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
        InvokeRepeating("Live", 0.2f, 1 / speedOfLiving);
        food = maxFood;
        foodBar.maxValue = food;
        foodBar.value = food;
        chitin = maxChitin;
        chitinBar.maxValue = chitin;
        chitinBar.value = chitin;
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            ExecuteEvents.Execute(PauseButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    void FixedUpdate()
    {
        if (transform.position.y < depthForDeath)
        {
            GameOver();
        }

        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        grounded = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;

        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement(move, false);

        move = Vector2.up * deltaPosition.y;

        Movement(move, true);
    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance)
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for (int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }


        }

        rb2d.position = rb2d.position + move.normalized * distance;
    }
    
    void Live()
    {
        food -= foodSubtraction;
        if (food > 0)
        {
            foodBar.value = food;
        }
        else
        {
            GameOver();
        }
        chitin -= chitinSubtraction;
        if (chitin > 0)
        {
            chitinBar.value = chitin;
        }
        else
        {
            GameOver();
        }
    }

    void GameOver()
    {
        GameOverSound.Play();
        Time.timeScale = 0;
        GameOverMenu.SetActive(true);
        PauseButton.gameObject.SetActive(false);
    }

    void  GameWin()
    {
        GameWinSound.Play();
        Time.timeScale = 0;
        GameWinMenu.SetActive(true);
        PauseButton.gameObject.SetActive(false);
    }

    void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }
        
        bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < 0.01f));
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        targetVelocity = move * maxSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Algae"))
        {
            other.gameObject.SetActive(false);
            food += algaeFood;
            if (food > maxFood)
            {
                food = maxFood;
            }
            foodBar.value = food;
        }
        if (other.gameObject.CompareTag("Shrimp"))
        {
            other.gameObject.SetActive(false);
            chitin += shrimpChitin;
            if (chitin > maxChitin)
            {
                chitin = maxChitin;
            }
            chitinBar.value = chitin;
            food += shrimpFood;
            if (food > maxFood)
            {
                food = maxFood;
            }
            foodBar.value = food;
        }
        if (other.gameObject.CompareTag("Goal"))
        {
            GameWin();
        }
    }
}