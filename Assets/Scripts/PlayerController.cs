using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Horizontal Movement Settings")]
    //Determines speed while walking
    //Using SerializedField instead of public because we are only using walkSpeed in the PlayerController script
    //Public will allow walkSpeed to be available to all scripts
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]

    [Header("Vertical Movement Settings")]
    //Determines jump force
    [SerializeField] private float jumpForce = 45;

    //Helps time jumping better, especially when grounded
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;

    //Coyote time - that extra bit of time where you can still jump even if you're technically no longer on a platform
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;

    //Counter to check how many times we jumped while in the air
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;
    [Space(5)]

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    //Lets us know what layer we consider the ground
    [SerializeField] private LayerMask whatIsGround;
    [Space(5)]

    [Header("Dash Settings")]
    //Variables for dashing
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [SerializeField] GameObject dashEffect;
    [Space(5)]

    private bool canDash = true;
    //For dashing while midair
    private bool dashed;

    private float gravity;

    private bool attack = false;
    private float timeBetweenAttack, timeSinceAttack;
    [Header("Attack Settings")]
    [SerializeField] Transform SideAttackTransform;
    [SerializeField] Transform UpAttackTransform;
    [SerializeField] Transform DownAttackTransform;
    [SerializeField] Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] private float damage;
    [SerializeField] GameObject slashEffect;
    [Space(5)]

    [Header("Recoil Settings")]
    [SerializeField] private int recoilXSteps = 5;
    [SerializeField] private int recoilYSteps = 5;
    [SerializeField] private float recoilXSpeed = 100;
    [SerializeField] private float recoilYSpeed = 100;
    private int stepsXRecoiled, stepsYRecoiled;
    [Space(5)]

    [Header("Health Settings")]
    public int health;
    public int maxHealth;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] private float hitFlashSpeed;
    //Delegate can be used to call multiple methods. We can use this to not only increase hearts, but decrease them as well
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallBack;
    private float healTimer;
    [SerializeField] private float timeToHeal;
    [Space(5)]


    //Reference the PlayerStateList
    [HideInInspector] public PlayerStateList pState;
    //private Animator anim;
    //Will be accessible by all our scripts
    public static PlayerController Instance;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    //Helps get directional input from player
    private float xAxis, yAxis;
    //Variables to be used to modify timescale
    private bool restoreTime;
    private float restoreTimeSpeed;

    //Is called before the Start function
    private void Awake()
    {
        //If Instance variable has already been set AND this PlayerController is not that Instance, destroy the gameObject
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        //Otherwise, make this gameObject the PlayerController instance
        else
        {
            Instance = this;
        }

        Health = maxHealth;
    }

    void Start()
    {
        //Lets us get the playerstate
        pState = GetComponent<PlayerStateList>();
        //Assigns to player character
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        //anim = GetComponent<Animator>();

        gravity = rb.gravityScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        UpdateJumpVariables();
        //Movement does not get called if we are in the middle of dashing
        if (pState.dashing) return;
        Flip();
        Move();
        Jump();
        StartDash();
        Attack();
        RestoreTimeScale();
        FlashWhiteInvincible();
        Heal();
    }

    //FixedUpdate is affected by timescale, whereas Update is not. Update will run every frame regardless of whatever is going on
    private void FixedUpdate()
    {
        if (pState.dashing) return;
        Recoil();
    }

    void GetInputs()
    {
        //Gets the horizontal axis input
        xAxis = Input.GetAxisRaw("Horizontal");
        //Gets the vertical axis input
        yAxis = Input.GetAxisRaw("Vertical");
        //Gets the attack input
        attack = Input.GetButtonDown("Attack");
    }

    void Flip()
    {
        //Flips the sprite based on movement direction
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);

            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);

            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        //Will move the character horizontally at walkSpeed without changing vertical speed
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        //anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded())
        {
            dashed = false;
        }
    }

    //IEnumerator will go step by step and perform each line
    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        //anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        //Will play the dash effect while dashing on the ground
        //if (Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        //Check to see if we can attack again after we attack
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            //anim.SetTrigger("Attacking")

            //Sets the default attack for if the player is attacking to the side or attacking while grounded
            if (yAxis == 0 || yAxis < 0 && Grounded())
            {
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, recoilXSpeed);
                //Instantiate(slashEffect, SideAttackTransform);
            }
            //Hits up if player is pressing up while attacking
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, recoilYSpeed);
                //SlashEffectAtAngle(slashEffect, 90, UpAttackTransform);
            } 
            else if (yAxis < 0 && !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, recoilYSpeed);
                //SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
            }
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        List<Enemy> hitEnemies = new List<Enemy>();

        //Runs the log if there is actually anything to hit in our areas
        if (objectsToHit.Length > 0)
        {
            //Debug.Log("Hit!");
            _recoilDir = true;
        }

        //Goes through list of objectsToHit
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            //If there IS an enemy to hit (aka list is NOT null), we do damage and knockback to them
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                //(transform.position - objectsToHit[i].transform.position).normalized determines the direction of the player's hit
                //objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);

                //To prevent enemies with multiple colliders from being hit multiple times from a single hit, we use this instead
                //Checks to see if the collider we are about to hit is part of an enemy we already hit
                //If not, we apply the hit then add it to our list of enemies hit during this attack
                Enemy e = objectsToHit[i].GetComponent<Enemy>();
                if(e && !hitEnemies.Contains(e))
                {
                    e.EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);
                    hitEnemies.Add(e);
                }
            }
        }
    }

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void Recoil()
    {
        //If facing right, they will recoil left, otherwise recoil right
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        //Using < 0 because if that is the case, the player is attacking downwards
        if (pState.recoilingY)
        {
            //Makes it so gravity does not affect the recoil
            rb.gravityScale = 0;
            //Attacking downwards
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            //Attacking upwards
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }

            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        //stop recoil
        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }

        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }
    
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

    public void TakeDamage(float _damage)
    {
        Health -= Mathf.RoundToInt(_damage);
        StartCoroutine(StopTakingDamage());
    }

    //Our iframes that trigger once we get hurt
    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        //anim.SetTrigger("TakeDamage");
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    //Goes from white to black to white again and again so long as parameters are fulfilled
    void FlashWhiteInvincible()
    {
        sr.material.color = pState.invincible ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;
    }

    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }
            else
            {
                //If our timescale somehow goes over 1, this sets it back to 1
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    public void HitStartTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        //Allows for the restore time speed to be different depending on the enemy that attacks the player
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;
        //If we have been attacked
        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }

    //Makes Health a Property and lets us get and set health as needed (health and Health are different in this case)
    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if (onHealthChangedCallBack != null)
                {
                    onHealthChangedCallBack.Invoke();
                }
            }
        }
    }

    void Heal()
    {
        if (Input.GetButton("Healing") && Health < maxHealth && !pState.jumping && !pState.dashing)
        {
            pState.healing = true;
            //anim.SetBool("Healing", true);

            healTimer += Time.deltaTime;
            if(healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }
        }
        else
        {
            pState.healing = false;
            healTimer = 0;
        }
    }

    //Prevents health from going above max and below min
    public bool Grounded()
    {
        //Checks if we are actually on the ground. Applies even if we are on an edge
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {
        //Allows to cancel jump while already jumping upwards
        //AKA variable jump height
        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

            pState.jumping = false;
        }

        if (!pState.jumping)
        {
            //Jumps if buffer counter is greater than 0 AND Grounded
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);

                pState.jumping = true;
            }
            else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                pState.jumping = true;

                airJumpCounter++;

                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            }
        }

        //anim.SetBool("Jumping", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            //Time.deltaTime = amount of time between each frame; In our case the counter goes down by 1 per second
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }
}
