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
    //Will be responsible to any updates to the heart UI. Delegate void can be used for multiple methods, in this case we are using it to increase and decrease our hearts on screen
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallBack;
    private float healTimer;
    [SerializeField] private float timeToHeal;
    [Space(5)]

    [Header("Mana Settings")]
    [SerializeField] private float mana;
    [SerializeField] private float manaDrainSpeed;
    [SerializeField] private float manaGain;

    private float castOrHealTimer;

    //Reference the PlayerStateList
    [HideInInspector] public PlayerStateList pState;
    Animator anim;
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
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //Lets us get the playerstate
        pState = GetComponent<PlayerStateList>();
        //Assigns to player character
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        anim = GetComponent<Animator>();

        gravity = rb.gravityScale;

        Mana = mana;
        Health = maxHealth;
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
        if (Input.GetButtonDown("Interact"))
        {
            Debug.Log("Interacting");
        }
        if (pState.cutscene) return;
        Debug.Log(pState.alive.ToString());
        if (pState.alive)
        {
            GetInputs();
        }
        UpdateJumpVariables();
        RestoreTimeScale();
        //Movement does not get called if we are in the middle of dashing
        if (pState.dashing) return;
        if (pState.alive)
        {
            Move();
            Heal();
            Flip();
            Jump();
            StartDash();
            Attack();
        }
        if (pState.healing) return;
        FlashWhileInvincible();
    }

    //FixedUpdate is affected by timescale, whereas Update is not. Update will run every frame regardless of whatever is going on
    private void FixedUpdate()
    {
        if (pState.dashing || pState.healing) return;
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

        if (Input.GetButton("Cast/Heal"))
        {
            castOrHealTimer += Time.deltaTime;
        }
        else
        {
            castOrHealTimer = 0;
        }
    }

    void Flip()
    {
        //Flips the sprite based on movement direction
        //Setting the x in the Vector2 to 3 because of how our model is scaled
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-3, transform.localScale.y);

            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(3, transform.localScale.y);

            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        if (pState.healing) rb.velocity = new Vector2(0, 0);
        //Will move the character horizontally at walkSpeed without changing vertical speed
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
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
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
        //Will play the dash effect while dashing on the ground
        //if (Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public IEnumerator WalkIntoANewScene(Vector2 _exitDir, float _delay)
    {
        //If exit direction is upwards
        if(_exitDir.y > 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        //If exit direction requires horizontal movement
        if(_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;

            Move();
        }

        Flip();
        yield return new WaitForSeconds(_delay);
        pState.cutscene = false;
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        //Check to see if we can attack again after we attack
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

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

        //Goes through list of objectsToHit
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            Enemy e = objectsToHit[i].GetComponent<Enemy>();
            if (e && !hitEnemies.Contains(e))
            {
                e.EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);
                hitEnemies.Add(e);

                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    Mana += manaGain;
                }
            }
            /*
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

                    //If what we hit is an enemy, we regain some mana
                    if (objectsToHit[i].CompareTag("Enemy"))
                    {
                        Mana += manaGain;
                    }
                }
            }
            */            
        }

        //Runs the log if there is actually anything to hit in our areas
        //Sets our recoil direction to true, causing recoil
        if (objectsToHit.Length > 0)
        {
            //Debug.Log("Hit!");
            _recoilDir = true;
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
        if (pState.alive)
        {
            Health -= Mathf.RoundToInt(_damage);

            if(Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(StopTakingDamage());
            }
        }
    }

    //Our iframes that trigger once we get hurt
    IEnumerator StopTakingDamage()
    {
        Debug.Log("StopTakingDamage Running");
        pState.invincible = true;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        //anim.SetTrigger("TakeDamage");
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    //Goes from white to black to white again and again so long as parameters are fulfilled
    void FlashWhileInvincible()
    {
        sr.material.color = pState.invincible ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;
    }

    void RestoreTimeScale()
    {
        Debug.Log("RestoreTimeScale Running");
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.unscaledDeltaTime * restoreTimeSpeed;
            }
            else
            {
                //If our timescale somehow goes over 1, this sets it back to 1
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        Debug.Log("HitStopTime Running");
        //Allows for the restore time speed to be different depending on the enemy that attacks the player
        restoreTimeSpeed = _restoreSpeed;
        //Time.timeScale = _newTimeScale;
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

        Time.timeScale = _newTimeScale;
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        yield return new WaitForSecondsRealtime(_delay);
        restoreTime = true;
    }

    IEnumerator Death()
    {
        pState.alive = false;
        //Make sure that time stop will not affect us since we got hit
        Time.timeScale = 1f;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("Death");

        yield return new WaitForSeconds(0.9f);
        StartCoroutine(UIManager.Instance.ActivateDeathScreen());
    }

    //Function to handle what happens after respawn
    public void Respawned()
    {
        if (!pState.alive)
        {
            pState.alive = true;
            Health = maxHealth;
            anim.Play("player_Idle");
        }
    }

    //Makes Health a Property and lets us get and set health as needed (health and Health are different in this case)
    int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                /* Commenting this out for now. This will be used for our hearts UI. Since we don't have the assets right now,
                 * this is causing everything to stop progressing past this point, causing our enemy infinite damage bug
                 * as well as our instant heal bug
                 */
                if (onHealthChangedCallBack != null)
                {
                    onHealthChangedCallBack.Invoke();
                }
            }
        }
    }

    float Mana
    {
        get { return mana; }
        set
        {
            //if mana stats change
            if (mana != value)
            {
                mana = Mathf.Clamp(value, 0, 1);
            }
        }
    }

    void Heal()
    {
        if (Input.GetButton("Cast/Heal") && castOrHealTimer > 0.05f && Health < maxHealth && Grounded() && Mana > 0 && !pState.jumping && !pState.dashing)
        {
            pState.healing = true;
            //anim.SetBool("Healing", true);

            healTimer += Time.deltaTime;
            if(healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }

            //drain mana while healing
            Mana -= Time.deltaTime * manaDrainSpeed;
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
        //Jumps if buffer counter is greater than 0 AND Grounded
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.jumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);

            pState.jumping = true;
        }
        
        if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
        {
            pState.jumping = true;

            airJumpCounter++;

            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        }

        //Allows to cancel jump while already jumping upwards
        //AKA variable jump height
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 3)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

            pState.jumping = false;
        }

        anim.SetBool("Jumping", !Grounded());
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
