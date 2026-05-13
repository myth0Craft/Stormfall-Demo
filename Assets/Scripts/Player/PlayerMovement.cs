using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public enum HorizontalState
{
    Idle,
    Walking,
    Dashing,
    Sprinting,
    ShieldSliding
}

public enum CombatState
{
    Idle,
    Attacking,
    Blocking
}

public enum VerticalState
{
    Idle,
    Jumping,
    Falling,
    StuckToWall
}

public class PlayerMovement : MonoBehaviour
{
    
    public static PlayerMovement instance { get; private set; }
    public HorizontalState currentHorizontalState;
    public VerticalState currentVerticalState;
    public CombatState currentCombatState;
    public Rigidbody2D body;
    private LayerMask groundLayer;
    private BoxCollider2D boxCollider;

    [SerializeField] private GameObject visual;

    public bool abilityDebug = false;

    private PlayerMeleeAttack playerMeleeAttack;

    private bool groundedThisFrame;

    public float horizontalInput { get ; private set; }

    public float verticalInput { get; private set; }

    private Coroutine jumpHoldRoutine;
    private Vector2 SlopeNormalPerp;

    //movement values
    private float speed = 4f;
    private float jumpStrength = 8f;

    private float baseGravity = 5f;
    private float lowJumpGravity = 7f;
    private float fallGravity = 10f;

    //ability usage/cooldown trackers
    private bool facingRight = true;
    private bool doubleJumpUsed = false;
    private bool dashUsed = false;


    //movement fine-tuning values
    private int maxJumpHoldFrames = 15;
    private float jumpIncreasePerFrameHeld = 0.5f;
    private int jumpHoldCounter = 0;

    private float jumpBufferTime = 0.1f;
    private float jumpBufferTimer = 0f;

    private float groundedRememberTime = 0.1f;
    private float groundedRememberTimer = 0f;
    private float wallRememberTime = 0.1f;
    private float wallRememberTimer = 0f;
    private float gravityMultiplier = 0.35f;
    private float accelGrounded = 40f;
    private float accelInAir = 25f;


    private float dashFrames = 0f;
    private float maxDashFrames = 15f;

    private float dashCooldown = 0;

    private float fallTime = 0;

    public float jumpTime { get; private set; } = 0f;


    //TODO: FIX TIME
    private float dashCooldownTime = 30f;


    //inputs
    private PlayerControls controls;

    //camera
    private CameraFollowObject cameraFollowObject;

    private ParticleSystem sprintParticles;

    private Coroutine shieldSlideCoroutine;


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        controls = PlayerData.getControls();
        groundLayer = LayerMask.GetMask("Ground");
        sprintParticles = GetComponentInChildren<ParticleSystem>();
        playerMeleeAttack = GetComponent<PlayerMeleeAttack>();



        controls.Player.Move.started += OnDirectionInput;
        controls.Player.Move.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>().x;
        controls.Player.Move.canceled += OnDirectionInputCancel;

        controls.Player.Move.performed += ctx => verticalInput = ctx.ReadValue<Vector2>().y;

        controls.Player.Jump.performed += OnJumpPressed;
        controls.Player.Jump.started += OnJumpHeld;
        controls.Player.Jump.canceled += NewOnJumpReleased;

        controls.Player.Dash.performed += OnDashPressed;
        controls.Player.Dash.canceled += OnSprintCanceled;

        controls.Player.Block.performed += OnBlockPressed;

        #if !UNITY_EDITOR
            gameObject.transform.position = new Vector2(PlayerData.posX, PlayerData.posY);
        #endif


        currentHorizontalState = HorizontalState.Idle;
        currentVerticalState = VerticalState.Idle;
        currentCombatState = CombatState.Idle;
        sprintParticles.Stop();

    }

    private void OnDashPressed(InputAction.CallbackContext context)
    {
        currentHorizontalState = HorizontalState.Dashing;
        dashFrames = maxDashFrames;
        PlayerAnimationManager.instance.Dash();
    }

    private void OnDirectionInputCancel(InputAction.CallbackContext context)
    {
        horizontalInput = 0;
        verticalInput = 0;
    }

    private void OnBlockPressed(InputAction.CallbackContext context)
    {
        if (PlayerData.shieldUnlocked || abilityDebug)
        {
            if (verticalInput < -0.1f)
            {
                ExecuteShieldBounce();
            } else if (verticalInput > 0.1f)
            {
                StartShieldSlide();
            } else
            {
                currentHorizontalState = HorizontalState.Idle;
                currentCombatState = CombatState.Blocking;
                StartCoroutine(BlockCoroutine());
            }
            PlayerAnimationManager.instance.Block();
        }
    }

    private void StartShieldSlide()
    {
        PlayerAnimationManager.instance.SetShieldSlide(true);

    }

    private IEnumerator BlockCoroutine()
    {
        yield return new WaitForSeconds(2f);
        currentCombatState = CombatState.Idle;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        if (PlayerData.sprintUnlocked || abilityDebug)
        {
            sprintParticles.Stop();
            currentHorizontalState = HorizontalState.Walking;
        }
        
    }

    private void OnSprintStarted()
    {
        if ( PlayerData.sprintUnlocked || abilityDebug)
        {
            sprintParticles.Play();
        }
    }

    private void NewOnJumpReleased(InputAction.CallbackContext context)
    {
        //currentVerticalState = VerticalState.Falling;

        if (body.linearVelocity.y > 0)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y * 0.4f);
            PlayerAnimationManager.instance.SetCapeJumpTrigger();
        }
    }

    private void OnJumpHeld(InputAction.CallbackContext context)
    {
        if (jumpHoldRoutine != null)
            StopCoroutine(jumpHoldRoutine);


        jumpHoldRoutine = StartCoroutine(JumpHeldCoroutine());
    }

    private IEnumerator JumpHeldCoroutine()
    {
        while (currentVerticalState == VerticalState.Jumping && jumpHoldCounter > 0)
        {
            body.linearVelocity += Vector2.up * (jumpStrength * jumpIncreasePerFrameHeld) * Time.fixedDeltaTime;
            jumpHoldCounter--;
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnJumpPressed(InputAction.CallbackContext context)
    {
        jumpBufferTimer = jumpBufferTime;
    }

    private void BeginJump()
    {
        

        if (StuckToWallBuffered() && !IsGroundedBuffered() && (PlayerData.wallJumpUnlocked || abilityDebug))
        {
            jumpHoldCounter = maxJumpHoldFrames;
            ExecuteWallJump();
            currentVerticalState = VerticalState.Jumping;
        }
        else
        {
            jumpHoldCounter = maxJumpHoldFrames;
            Jump();
            currentVerticalState = VerticalState.Jumping;

        }
    }

    private void OnDirectionInput(InputAction.CallbackContext context)
    {
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            currentHorizontalState = HorizontalState.Walking;
        }
    }

    private float NewGetGravity()
    {
        float finalGravity = baseGravity;

        //prevent player from sliding on slopes
        if (IsOnSlope() && Mathf.Abs(horizontalInput) < 0.1f && groundedThisFrame && currentVerticalState != VerticalState.Jumping)
        {
            finalGravity = 0f;
            return finalGravity;
        }

        if (currentVerticalState == VerticalState.Jumping)
        {
            //finalGravity = lowJumpGravity;
        } else if (currentVerticalState == VerticalState.Falling) {
            finalGravity = fallGravity;
        }

        //if stuck to wall, slow gravity for wall slide
        if (currentVerticalState == VerticalState.StuckToWall)
        {
            finalGravity *= 0.1f;
        }

        //if dashing in midair, slow gravity
        if (currentHorizontalState == HorizontalState.Sprinting)
        {
            finalGravity *= 0.6f;
        }

        return finalGravity;
    }



    private void ApplyVerticalMovement()
    {

        if (jumpBufferTimer > 0f)
        {
            bool canJump =
                groundedThisFrame ||
                groundedRememberTimer > 0f ||
                (!doubleJumpUsed && (PlayerData.doubleJumpUnlocked || abilityDebug));

            bool canWallJump =
                StuckToWallBuffered() &&
                !groundedThisFrame;

            if (canJump || canWallJump)
            {
                BeginJump();
                jumpBufferTimer = 0f;
            }
        }

        UpdateVerticalState();

        body.gravityScale = NewGetGravity() * gravityMultiplier;

        if (currentVerticalState == VerticalState.Jumping)
        {

        } else if (currentVerticalState == VerticalState.Falling)
        {

        } else if (currentVerticalState == VerticalState.StuckToWall)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, Mathf.Max(body.linearVelocity.y, -5f));
            return;   
        }

        body.linearVelocity = new Vector2(body.linearVelocity.x, Mathf.Max(body.linearVelocity.y, -15f));
    }



    private void UpdateVerticalState()
    {
        if (StuckToWallBuffered() && (PlayerData.wallJumpUnlocked || abilityDebug))
        {
            currentVerticalState = VerticalState.StuckToWall;
        }
        else if (IsGroundedBuffered())
        {
            currentVerticalState = VerticalState.Idle;
        }
        else if (body.linearVelocity.y > 0)
        {
            currentVerticalState = VerticalState.Jumping;
        }
        else
        {
            currentVerticalState = VerticalState.Falling;
        }
    }

    private bool IsMidairState()
    {
        return currentVerticalState == VerticalState.Falling || currentVerticalState == VerticalState.Jumping;
    }

    private void ApplyHorizontalMovement()
    {
        if (currentHorizontalState == HorizontalState.Dashing)
        {

            if (dashFrames > 0)
            {
                ApplyDashMovement();
                dashFrames--;

            } else
            {
                currentHorizontalState = HorizontalState.Sprinting;
                OnSprintStarted();
            }

            
        }


        if (Mathf.Abs(horizontalInput) > 0.01f)
            TurnSprite();

        if (currentHorizontalState == HorizontalState.Idle || currentCombatState == CombatState.Blocking)
        {
            StopMoving();
            return;
        }

        

        if (currentHorizontalState == HorizontalState.Walking)
        {
            ApplyNormalHorizontalMovement(1f);
        } else if (currentHorizontalState == HorizontalState.Sprinting)
        {
            ApplyNormalHorizontalMovement(1.70f);
        }
    }

    private void ApplyDashMovement()
    {
        float xVel = getFacingDirection() ? 10 : -10;
        body.linearVelocity = new Vector2(xVel, 0);
    }

    private void StopMoving()
    {
        float accel = IsMidairState() ? accelInAir : accelGrounded;

        float newVelX = Mathf.MoveTowards(body.linearVelocity.x, 0, accel * Time.fixedDeltaTime * 2);

        body.linearVelocity = new Vector2(newVelX, body.linearVelocity.y);
    }

    private void ApplyNormalHorizontalMovement(float bonusSpeed)
    {
        if (IsOnSlope())
        {
            ApplySlopeHorizontalMovement(bonusSpeed);
            return;
        }

        float accel = IsMidairState() ? accelInAir : accelGrounded;

        float newVelX = Mathf.MoveTowards(body.linearVelocity.x, horizontalInput * speed * bonusSpeed, accel * Time.fixedDeltaTime * 2);

        body.linearVelocity = new Vector2(newVelX, body.linearVelocity.y);
    }

    private void ApplySlopeHorizontalMovement(float bonusSpeed)
    {

        float newVelX = Mathf.MoveTowards(body.linearVelocity.x, horizontalInput * speed * bonusSpeed, accelGrounded * Time.fixedDeltaTime * 2);

        if (IsFacingSlope())
        {

            if (Mathf.Abs(horizontalInput) < 0.1f && currentVerticalState != VerticalState.Jumping)
            {
                body.linearVelocity = new Vector2(0f, 0f);
                return;
            }

            if (getFacingDirection())
            {
                body.linearVelocity = new Vector2(Mathf.Min(newVelX * 2, 4.1f) * (SlopeNormalPerp.x * -1.1f), body.linearVelocity.y);

            }
            else
            {
                body.linearVelocity = new Vector2(newVelX * (SlopeNormalPerp.x * -1.1f), body.linearVelocity.y);
            }
        }
        else
        {

            body.linearVelocity = new Vector2(newVelX * SlopeNormalPerp.x * -1, body.linearVelocity.y);
        }


        if (Mathf.Abs(horizontalInput) < 0.1f && currentVerticalState != VerticalState.Jumping && body.linearVelocity.y <= 0.01f && Mathf.Abs(SlopeNormalPerp.x) != 1f)
        {
            body.linearVelocity = new Vector2(0f, 0f);
            return;
        }
    }



    private void OnDestroy()
    {
        controls.Player.Jump.started -= OnJumpPressed;
        controls.Player.Jump.canceled -= NewOnJumpReleased;
    }


    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }



    private void FixedUpdate()
    {

        groundedThisFrame = IsGrounded();

        UpdateTimers();

        ApplyHorizontalMovement();

        ApplyVerticalMovement();
    }


    public float getDashFrames()
    {
        return dashFrames;
    }

    public IEnumerator MoveHorizontalToPosition(float xPos)
    {
        while (Mathf.Abs(transform.position.x - xPos) > 0.01f)
        {
            Vector3 pos = transform.position;

            pos.x = Mathf.MoveTowards(pos.x, xPos, speed * Time.deltaTime);

            transform.position = pos;

            yield return null;
        }
    }

    public Vector3 getLinearVelocity()
    {
        return body.linearVelocity;
    }

    public bool getFacingDirection()
    {
        return facingRight;
    }

    public void TurnSprite()
    { 
        bool shouldFaceRight = horizontalInput > 0;

        //turn logic - only executes if player is not currently attacking. If the player is in midair, turn logic still applies regardless of attack state.
        if (shouldFaceRight != facingRight && !playerMeleeAttack.isMidAttack 
            && !playerMeleeAttack.attackHitboxActive)
        {

            //rotates player 180 on y
            Vector3 rot = visual.transform.rotation.eulerAngles;
            rot.y = shouldFaceRight ? 0f : 180f;
            visual.transform.rotation = Quaternion.Euler(rot);

            facingRight = shouldFaceRight;

            PlayerAnimationManager.instance.SetTurnTrigger();
        }
    }

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
        boxCollider.bounds.center,
        boxCollider.bounds.size,
        0f,
        Vector2.down,
        0.1f,
        groundLayer
        );
        if (hit.collider != null)
        {
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }


        return hit.collider != null;
    }

    
    public bool IsOnSlope()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
        boxCollider.bounds.center,
        boxCollider.bounds.size,
        0f,
        Vector2.down,
        0.1f,
        groundLayer
        );



        if (hit.collider != null)
        {
            SlopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            return hit.collider.CompareTag("Slope");
        }
        else
        {
            return false;
        }
    }

    public bool IsFacingSlope()
    {

        Vector2 direction = getFacingDirection() ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, direction, 0.1f, groundLayer);
        if (hit.collider != null)
        {
            return hit.collider.CompareTag("Slope");
        }
        else
        {
            return false;
        }
    }


    //checks if player was on ground in last 0.1s
    public bool IsGroundedBuffered()
    {
        return groundedRememberTimer > 0f;
    }

    private bool StuckToWall()
    {
        Vector2 direction = getFacingDirection() ? Vector2.right : Vector2.left;

        bool isTouchingWall = Physics2D.Raycast(boxCollider.bounds.center, direction, boxCollider.bounds.size.x - 0.05f, groundLayer);

        //return hit.collider != null
        return isTouchingWall && body.linearVelocityY <= 0.1 && (PlayerData.wallJumpUnlocked || abilityDebug);
    }

    public bool StuckToWallBuffered()
    {
        return wallRememberTimer > 0f;
    }

    //applies upwards jump motion for jumps and double jumps
    private void Jump()
    {

        //jumping starting from the ground
        if (IsGroundedBuffered())
        {
            //jump logic
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpStrength);
            jumpHoldCounter = maxJumpHoldFrames;


            //starting from midair (double jump)
        }
        else if (!doubleJumpUsed && (PlayerData.doubleJumpUnlocked || abilityDebug))
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpStrength);
            jumpHoldCounter = maxJumpHoldFrames;
            doubleJumpUsed = true;
        }
    }

    //makes the player jump off of a wall
    private void ExecuteWallJump()
    {
        if (PlayerData.wallJumpUnlocked || abilityDebug)
        {

            float wallJumpHorizontalForce = facingRight ? -10f : 10f;
            body.linearVelocity = new Vector2(wallJumpHorizontalForce, jumpStrength * 1f);
        }
    }

    private void ExecuteShieldBounce()
    {
        if (PlayerData.shieldBounceUnlocked || abilityDebug)
        {

            float wallJumpHorizontalForce = Mathf.Abs(horizontalInput) < 0.1f ? 0 : facingRight ? 15f : -15f;
            body.linearVelocity = new Vector2(wallJumpHorizontalForce, jumpStrength * 1.3f);
        }
    }

    private void UpdateTimers()
    {
        if (groundedThisFrame)
            groundedRememberTimer = groundedRememberTime;
        else
            groundedRememberTimer -= Time.fixedDeltaTime;

        if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.fixedDeltaTime;
        } 

        if (StuckToWall())
            wallRememberTimer = wallRememberTime;
        else
            wallRememberTimer -= Time.fixedDeltaTime;
    }
}