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
    public Rigidbody2D body;
    private LayerMask groundLayer;
    private BoxCollider2D boxCollider;

    [SerializeField] private GameObject visual;

    public bool abilityDebug = false;

    private PlayerMeleeAttack playerMeleeAttack;

    private bool groundedThisFrame;

    private float horizontalInput;
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
    private float gravityMultiplier = 0.4f;
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
    public float horizontalMovement { get; private set; }
    private float previousHorizontalMovement = 0;
    private bool jumpPressed;
    private bool dashPressed;
    public bool dashHeld { get; private set; }
    private bool jumpHeld;
    private bool wasJumpHeld;

    public bool isWallJumping = false;

    //camera
    private CameraFollowObject cameraFollowObject;
    private float fallSpeedYDampingChangeThreshold;

    private ParticleSystem sprintParticles;


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



        controls.Player.Move.performed += ctx => horizontalMovement = ctx.ReadValue<Vector2>().x;
        controls.Player.Move.performed += OnHorizontalInput;
        controls.Player.Move.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>().x;
        controls.Player.Move.canceled += ctx => horizontalInput = 0f;
        controls.Player.Move.canceled += ctx => horizontalMovement = 0f;

        controls.Player.Jump.performed += ctx => jumpPressed = true;
        controls.Player.Dash.performed += ctx => dashPressed = true;
        controls.Player.Dash.started += ctx => dashHeld = true;
        controls.Player.Dash.canceled += ctx => dashHeld = false;


        //controls.Player.Jump.canceled += ctx => jumpHeld = false;
        //controls.Player.Jump.started += ctx => jumpHeld = true;
        controls.Player.Jump.performed += OnJumpPressed;
        controls.Player.Jump.started += OnJumpHeld;
        controls.Player.Jump.canceled += NewOnJumpReleased;

        #if !UNITY_EDITOR
            gameObject.transform.position = new Vector2(PlayerData.posX, PlayerData.posY);
        #endif


        currentHorizontalState = HorizontalState.Idle;
        currentVerticalState = VerticalState.Idle;

    }

    

    private void NewOnJumpReleased(InputAction.CallbackContext context)
    {
        currentVerticalState = VerticalState.Falling;

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
        jumpHoldCounter = maxJumpHoldFrames;
        

        if (StuckToWallBuffered() && !IsGroundedBuffered() && (PlayerData.wallJumpUnlocked || abilityDebug))
        {
            ExecuteWallJump();
            currentVerticalState = VerticalState.Jumping;
        }
        else
        {
            Jump();
            currentVerticalState = VerticalState.Jumping;
        }
    }

    private void OnHorizontalInput(InputAction.CallbackContext context)
    {
        currentHorizontalState = HorizontalState.Walking;
    }

    private float NewGetGravity()
    {
        float finalGravity = baseGravity;

        //prevent player from sliding on slopes
        if (IsOnSlope() && currentHorizontalState == HorizontalState.Idle && groundedThisFrame && currentVerticalState != VerticalState.Jumping)
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
        if (currentHorizontalState == HorizontalState.Idle)
        {
            return;
        }

        if (Mathf.Abs(horizontalInput) > 0.01f)
            TurnSprite();

        if (currentHorizontalState == HorizontalState.Walking)
        {
            ApplyNormalHorizontalMovement(1f);
        } else if (currentHorizontalState == HorizontalState.Sprinting)
        {
            ApplyNormalHorizontalMovement(1.70f);
        } else if (currentHorizontalState == HorizontalState.Dashing)
        {
            ApplyDashMovement();
        }
    }

    private void ApplyDashMovement()
    {
        float xVel = getFacingDirection() ? 10 : -10;
        body.linearVelocity = new Vector2(xVel, 0);
    }

    private void ApplyNormalHorizontalMovement(float bonusSpeed)
    {
            if (IsOnSlope())
            {
                ApplySlopeHorizontalMovement(bonusSpeed);
                return;
            }

        float accel = IsMidairState() ? accelInAir : accelGrounded;

        float newVelX = Mathf.MoveTowards(body.linearVelocity.x, horizontalInput * speed * bonusSpeed, accel * Time.deltaTime * 2);

        body.linearVelocity = new Vector2(newVelX, body.linearVelocity.y);
    }

    private void ApplySlopeHorizontalMovement(float bonusSpeed)
    {

        float newVelX = Mathf.MoveTowards(body.linearVelocity.x, horizontalInput * speed * bonusSpeed, accelGrounded * Time.deltaTime * 2);

        if (IsFacingSlope())
        {

            if (!(Mathf.Abs(horizontalInput) > 0.1f && body.linearVelocity.y >= -0.1f))
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


        if (Mathf.Abs(horizontalInput) < 0.1f && jumpPressed == false && body.linearVelocity.y <= 0.01f && Mathf.Abs(SlopeNormalPerp.x) != 1f)
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

    private void OldUpdate()
    {


        //resets double jump if player is on ground
        if (IsGroundedBuffered() || StuckToWallBuffered())
        {
            doubleJumpUsed = false;
            dashUsed = false;
        }

        //--- OLD ANIMATION STUFF. NOW LOCATED IN PLAYER ANIMATION MANAGER ---

        /*if (body.linearVelocity.y < fallSpeedYDampingChangeThreshold && CameraManager.instance.isLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if (body.linearVelocity.y >= 0f && !CameraManager.instance.isLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }*/
        //weaponsAnim.SetBool("moving", (horizontalMovement > 0.01f || horizontalMovement < -0.01f) && IsGroundedBuffered());
        /*capeAnim.SetBool("moving", (body.linearVelocity.x < -3f || body.linearVelocity.x > 3f) && !StuckToWallBuffered());
        bodyAnim.SetBool("moving", (horizontalMovement > 0.01f || horizontalMovement < -0.01f));
        //armsAnim.SetBool("moving", (horizontalMovement > 0.01f || horizontalMovement < -0.01f) && IsGroundedBuffered());
        legsAnim.SetBool("moving", (horizontalMovement > 0.01f || horizontalMovement < -0.01f));
        
        

        if (PlayerData.swordUnlocked)
        {
            swordAnim.SetBool("moving", (horizontalMovement > 0.01f || horizontalMovement < -0.01f) && IsGroundedBuffered());
        } else
        {
            disableSword();
        }

        if (PlayerData.shieldUnlocked)
        {
            shieldAnim.SetBool("moving", (horizontalMovement > 0.01f || horizontalMovement < -0.01f) && IsGroundedBuffered());
        }
        else
        {
            disableShield();
        }*/


    }


    private void OldFixedUpdate()
    {

        UpdateTimers();

        if (dashFrames > 0)
        {
            dashFrames--;
        }


        if (dashCooldown > 0)
        {
            dashCooldown--;
        }

        //Base left/right movement triggered per frame.
        MoveHorizontal();

        previousHorizontalMovement = horizontalMovement;

        //exits dash/sprint input when hitting a wall.
        if (StuckToWallBuffered())
        {
            dashHeld = false;
            dashPressed = false;
            dashFrames = 0;
            dashCooldown = 15;
        }

        //prevents dash inputs pressed while in midair from being activated when the player hits the ground.
        if (!IsGroundedBuffered() && dashHeld == false)
        {
            dashPressed = false;
        }

        //jumping

        if (jumpPressed)
        {
            jumpBufferTimer = jumpBufferTime;
            jumpPressed = false;
        }
        else
        {
            jumpBufferTimer -= Time.fixedDeltaTime;
        }



        if (jumpBufferTimer > 0f)
        {
            if (StuckToWallBuffered() && !IsGroundedBuffered() && (PlayerData.wallJumpUnlocked || abilityDebug))
            {
                ExecuteWallJump();
                jumpBufferTimer = 0f;
                isWallJumping = true;


            }
            else
            {
                Jump();
                jumpBufferTimer = 0f;
            }
        }

        ApplyJumpHold();

        //activates when player releases jump input. Immediatly slows down vertical velocity so that the player can control jump height precisely.
        if (wasJumpHeld && !jumpHeld)
        {
            OnJumpReleased();
            isWallJumping = false;
        }

        wasJumpHeld = jumpHeld;

        //dashing
        if (dashPressed && !dashUsed && !playerMeleeAttack.isMidAttack && !playerMeleeAttack.attackHitboxActive && (PlayerData.dashUnlocked || abilityDebug))
        {

            if (dashCooldown <= 0)

            {

                PlayerAnimationManager.instance.Dash();
                dashCooldown = dashCooldownTime;
                dashFrames = maxDashFrames;
                dashPressed = false;
                //playerMeleeAttack.attackCooldownTimer = 0f;
                GlobalHitstopManager.DoHitstop(0.02f);
            }
            //if (IsGroundedBuffered())
            //{
            //    dashPressed = false;
            //}
            dashPressed = false;
        }



        //apply current gravity
        body.gravityScale = getGravity() * gravityMultiplier;
        if (StuckToWallBuffered() && (PlayerData.wallJumpUnlocked || abilityDebug))
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, Mathf.Max(body.linearVelocity.y, -5f));

        }
        else
            body.linearVelocity = new Vector2(body.linearVelocity.x, Mathf.Max(body.linearVelocity.y, -15f));

        if (!IsGroundedBuffered() && !StuckToWallBuffered() && body.linearVelocity.y <= -0.1f)
        {
            fallTime++;
        }
        else
        {
            fallTime = 0f;
        }

        //float reqFallTime = Math.Abs(body.linearVelocity.x) > 0.1f ? 0f : 18f;


        if (!IsGroundedBuffered() && !StuckToWallBuffered())
        {
            jumpTime++;
        }
        else
        {
            jumpTime = 0f;
        }













        //emit sprint particles while sprinting
        if (IsGroundedBuffered() && dashHeld && (PlayerData.sprintUnlocked || abilityDebug))
        {
            sprintParticles.enableEmission = true;
        }
        else
        {
            sprintParticles.enableEmission = false;
        }

        /*if (IsOnSlope() && (Mathf.Abs(horizontalMovement) <= 0.1f || IsFacingSlope() && body.linearVelocity.x > 0.1f) && IsGrounded() && !jumpPressed && !(jumpHeld && jumpHoldCounter > 0))
        {
            body.linearVelocityY = 0f;
        }*/
    }



    public float getDashFrames()
    {
        return dashFrames;
    }



    //moves the player left or right
    private void MoveHorizontal()
    {
        /*float targetSpeed = input * speed;
        body.linearVelocity = new Vector2(targetSpeed, body.linearVelocity.y);*/


        //check for sprite turning every frame of movement


        //if dashing, increase horizontal velocity to 10
        if (dashFrames > 0 && !StuckToWallBuffered() && (PlayerData.dashUnlocked || abilityDebug))
        {

            dashUsed = true;
            float xVel = getFacingDirection() ? 10 : -10;
            body.linearVelocity = new Vector2(xVel, 0);
        }
        else
        {
            if (Mathf.Abs(horizontalMovement) > 0.01f)
                TurnSprite();

            float xMultiplier = 1f;
            //if sprinting, increase velocity.
            if (PlayerData.sprintUnlocked || abilityDebug)
            {
                xMultiplier = dashHeld ? 1.70f : 1;
            }
            /* if (!dashUsed && dashFrames > 0)
             {
                 multiplier = 20;
             print("dashed");
             }*/


            float accel = IsGroundedBuffered() ? accelGrounded : accelInAir;

            float newVelX = Mathf.MoveTowards(body.linearVelocity.x, horizontalMovement * speed * xMultiplier, accel * Time.deltaTime * 2);
            //float newVelX = horizontalMovement * speed * xMultiplier;

            if (IsOnSlope() && IsGrounded())
            {
                if (IsFacingSlope())
                {

                    if (!(Mathf.Abs(horizontalMovement) > 0.1f && body.linearVelocity.y >= -0.1f))
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


                if (Mathf.Abs(horizontalMovement) < 0.1f && jumpPressed == false && body.linearVelocity.y <= 0.01f && Mathf.Abs(SlopeNormalPerp.x) != 1f)
                {
                    body.linearVelocity = new Vector2(0f, 0f);
                    return;
                }
            }
            else
            {
                body.linearVelocity = new Vector2(newVelX, body.linearVelocity.y);
            }


        }
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
        bool shouldFaceRight = horizontalMovement > 0;

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

    //returns the desired gravity effect. Changes based on player jump state or if they're on a wall
    private float getGravity()
    {



        float finalGravity = baseGravity;

        //prevent player from sliding on slopes
        if (IsOnSlope() && Mathf.Abs(horizontalMovement) <= 0.1f && IsGrounded() && !jumpPressed && !(jumpHeld && jumpHoldCounter > 0))
        {
            finalGravity = 0f;
        }

        //Starting a jump cycle
        if (body.linearVelocity.y > 0 && !jumpHeld)
            finalGravity = lowJumpGravity;
        //Falling
        else if (body.linearVelocity.y < 0 && jumpTime > 0f)
            finalGravity = fallGravity;


        //if stuck to wall, slow gravity for wall slide
        if (StuckToWallBuffered() && body.linearVelocityY <= 0f && (PlayerData.wallJumpUnlocked || abilityDebug))
        {
            finalGravity *= 0.1f;
        }







        //if dashing in midair, slow gravity
        if (dashHeld && body.linearVelocity.y < 0 && (PlayerData.dashUnlocked || abilityDebug))
        {
            finalGravity *= 0.6f;
        }

        //if dashing into a wall, do not apply gravity
        if (dashFrames > 0f && !StuckToWallBuffered())
        {
            finalGravity = 0f;
        }



        return finalGravity;


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

    //continue to apply force if jump is held past the first frame.
    private void ApplyJumpHold()
    {
        if (jumpHeld && jumpHoldCounter > 0)
        {
            body.linearVelocity += Vector2.up * (jumpStrength * jumpIncreasePerFrameHeld) * Time.fixedDeltaTime;
            jumpHoldCounter--;
        }
    }

    //immediatly slow down y velocity when jump is released or exceeds maximum height. This allows player to have fine control over jump height
    private void OnJumpReleased()
    {
        if (body.linearVelocity.y > 0)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y * 0.4f);
            PlayerAnimationManager.instance.SetCapeJumpTrigger();
        }
    }




    //makes the player jump off of a wall
    private void ExecuteWallJump()
    {
        if (PlayerData.wallJumpUnlocked || abilityDebug)
        {

            float wallJumpHorizontalForce = facingRight ? -5.5f : 5.5f;
            body.linearVelocity = new Vector2(wallJumpHorizontalForce, jumpStrength * 1.3f);
        }
    }



    private void UpdateTimers()
    {
        if (groundedThisFrame)
            groundedRememberTimer = groundedRememberTime;
        else
            groundedRememberTimer -= Time.fixedDeltaTime;

        if (StuckToWall())
            wallRememberTimer = wallRememberTime;
        else
            wallRememberTimer -= Time.fixedDeltaTime;
    }
}