using System;
using System.Collections;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{

    public static PlayerMovement instance {  get; private set; }
    public Rigidbody2D body;
    private LayerMask groundLayer;
    private BoxCollider2D boxCollider;

    [SerializeField] private GameObject visual;

    public bool abilityDebug = false;

    private PlayerMeleeAttack playerMeleeAttack;

    private Vector3 sizeScale;
    


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

            //gets values from unity
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        controls = PlayerData.getControls();
        sizeScale = transform.localScale;
        groundLayer = LayerMask.GetMask("Ground");
        sprintParticles = GetComponentInChildren<ParticleSystem>();
        playerMeleeAttack = GetComponent<PlayerMeleeAttack>();
        


        //maps controls
        controls.Player.Move.performed += ctx => horizontalMovement = ctx.ReadValue<Vector2>().x;
        controls.Player.Move.canceled += ctx => horizontalMovement = 0f;

        controls.Player.Jump.performed += ctx => jumpPressed = true;
        controls.Player.Dash.performed += ctx => dashPressed = true;
        controls.Player.Dash.started += ctx => dashHeld = true;
        controls.Player.Dash.canceled += ctx => dashHeld = false;


        controls.Player.Jump.canceled += ctx => jumpHeld = false;
        controls.Player.Jump.started += ctx => jumpHeld = true;

        #if !UNITY_EDITOR
            gameObject.transform.position = new Vector2(PlayerData.posX, PlayerData.posY);
        #endif
    }

    private void Start()
    {
        //fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampingChangeThreshold;
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }


    private void Update()
    {
        //resets double jump if player is on ground
        if (IsGroundedBuffered() || StuckToWallBuffered())
        {
            doubleJumpUsed = false;
            dashUsed = false;
        }



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


    private void FixedUpdate()
    {

        if (dashFrames > 0)
        {
            dashFrames--;
        }

        
        if (dashCooldown > 0)
        {
            dashCooldown--;
        }

        //base left/right movement triggered per frame
        MoveHorizontal();

        previousHorizontalMovement = horizontalMovement;

        //exits dash/sprint input when hitting a wall
        if (StuckToWallBuffered())
        {
            dashHeld = false;
            dashPressed = false;
            dashFrames = 0;
            dashCooldown = 15;
        }

        //prevents dash inputs pressed while in midair from being activated when the player hits the ground
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
            if (StuckToWallBuffered() && !IsGroundedBuffered() && (PlayerData.wallJumpUnlocked || abilityDebug)) { 
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

        //activates when player releases jump input
        if (wasJumpHeld && !jumpHeld)
        {
            OnJumpReleased();
            isWallJumping = false;
        }

        wasJumpHeld = jumpHeld;

        //dashing
        if (dashPressed && !dashUsed && !playerMeleeAttack.isMidAttack && playerMeleeAttack.getAttackTimerTime() <= 0 && (PlayerData.dashUnlocked || abilityDebug))
        {

            if (dashCooldown <= 0)

            {

                PlayerAnimationManager.instance.Dash();
                dashCooldown = dashCooldownTime;
                dashFrames = maxDashFrames;
                dashPressed = false;
                playerMeleeAttack.attackCooldownTimer = 0f;
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

        } else
            body.linearVelocity = new Vector2(body.linearVelocity.x, Mathf.Max(body.linearVelocity.y, -15f));

        if (!IsGroundedBuffered() && !StuckToWallBuffered() && body.linearVelocity.y <= -0.1f)
        {
            fallTime++;
        } else
        {
            fallTime = 0f;
        }

        float reqFallTime = Math.Abs(body.linearVelocity.x) > 0.1f ? 0f : 18f;

        //capeAnim.SetBool("falling", fallTime > reqFallTime && !IsGroundedBuffered() && !StuckToWallBuffered());

        //if player is midair increase the midair time counter
        if (!IsGroundedBuffered() && !StuckToWallBuffered())
        {
            jumpTime++;
        } else
        {
            jumpTime = 0f;
        }

        

        

        

        

        

        

        //emit sprint particles while sprinting
        if (IsGroundedBuffered() && dashHeld && (PlayerData.sprintUnlocked || abilityDebug))
        {
            sprintParticles.enableEmission = true;
        } else
        {
            sprintParticles.enableEmission = false;
        }

        if (IsOnSlope() && (Mathf.Abs(horizontalMovement) <= 0.1f || IsFacingSlope() && body.linearVelocity.x > 0.1f) && IsGrounded() && !jumpPressed && !(jumpHeld && jumpHoldCounter > 0))
        {
            body.linearVelocityY = 0f;
        }
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
            //if sprinting, multiply velocity by 1.7
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

            //accelerate from current speed to target speed
            float newVelX = Mathf.MoveTowards(body.linearVelocity.x, horizontalMovement * speed * xMultiplier, accel * Time.fixedDeltaTime);

            if (IsOnSlope() && IsGrounded())
            {
                /*if (Mathf.Abs(horizontalMovement) <= 0.01f)
                {
                    body.linearVelocity = new Vector2(0.0f, body.linearVelocity.y);
                } else*/

                if (IsFacingSlope())
                {
                    body.linearVelocity = new Vector2(newVelX * SlopeNormalPerp.x * -1.1f, body.linearVelocity.y);
                }
                else
                {

                    body.linearVelocity = new Vector2(newVelX * SlopeNormalPerp.x * -1, body.linearVelocity.y);
                }
                
                
            } else
            {
                body.linearVelocity = new Vector2(newVelX, body.linearVelocity.y);
            }
            

        }
    }

    public IEnumerator MoveHorizontalToPosition(float xPos)
    {
        //print("target pos: " + xPos);
        //horizontalMovement = 0;
        //if (xPos < transform.position.x)
        //{
        //    horizontalMovement = -1;

        //    while (transform.position.x > xPos)
        //    {

        //        yield return null;

        //    }
        //} else if (xPos > transform.position.x)
        //{
        //    horizontalMovement = 1;
        //    while (transform.position.x < xPos)
        //    {

        //        yield return null;

        //    }
        //}

        //transform.SetPositionAndRotation(new Vector3(xPos, transform.position.y, transform.position.z), Quaternion.identity);




        //horizontalMovement = 0;


        while (Mathf.Abs(transform.position.x - xPos) > 0.01f)
        {
            Vector3 pos = transform.position;

            pos.x = Mathf.MoveTowards(pos.x, xPos, speed * Time.deltaTime);

            transform.position = pos;

            yield return null;
        }


    }

    //applies sideways motion for dashing
    private void Dash()
    {

        
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
        //Vector3 scale = transform.localScale;
        //scale.x = horizontalMovement > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        //scale.y = horizontalMovement > 0 ? 0f : 180f;
        //transform.localScale = scale;

        /*if (horizontalMovement > 0)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            facingRight = true;
        }else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            facingRight = false;
        }*/

        
        bool shouldFaceRight = horizontalMovement > 0;

        //turn logic - only executes if player is not currently attacking. If the player is in midair, turn logic still applies regardless of attack state
        if (shouldFaceRight != facingRight && !playerMeleeAttack.isMidAttack && playerMeleeAttack.getAttackTimerTime() <=0)
        {

            //rotates player
            Vector3 rot = visual.transform.rotation.eulerAngles;
            rot.y = shouldFaceRight ? 0f : 180f;
            visual.transform.rotation = Quaternion.Euler(rot);
            //body.transform.localScale = new Vector3(shouldFaceRight ? 1 : -1, 1, 1);

            facingRight = shouldFaceRight;

            PlayerAnimationManager.instance.SetTurnTrigger();

            
            //armsAnim.SetTrigger("turn");


        }
    }

    //returns the current gravity
    private float getGravity()
    {



        float finalGravity = baseGravity;

        //prevent player from sliding on slopes
        if (IsOnSlope() && Mathf.Abs(horizontalMovement) <= 0.1f && IsGrounded() && !jumpPressed && !(jumpHeld && jumpHoldCounter > 0))
        {
            finalGravity = 0f;
        }

        //starting a jump cycle
        if (body.linearVelocity.y > 0 && !jumpHeld)
            finalGravity = lowJumpGravity;
        //falling
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


    //checks if player is on ground
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

    private Vector2 SlopeNormalPerp;
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
        } else
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
        } else
        {
            return false;
        }


            /*RaycastHit2D hit = Physics2D.BoxCast(
                boxCollider.bounds.center,
                boxCollider.bounds.size,
                0f,
                direction,
                0.1f,
                groundLayer
            );*/

            //return hit.collider != null
    }


    //checks if player was on ground in last 0.1s
    public bool IsGroundedBuffered()
    {
        if (IsGrounded())
            groundedRememberTimer = groundedRememberTime;
        else
            groundedRememberTimer -= Time.deltaTime;

        return groundedRememberTimer > 0f;
    }

    //checks if player is attached to a wall
    private bool StuckToWall()
    {
        Vector2 direction = getFacingDirection() ? Vector2.right : Vector2.left;

        bool isTouchingWall = Physics2D.Raycast(boxCollider.bounds.center, direction, boxCollider.bounds.size.x - 0.05f, groundLayer);

        /*RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            direction,
            0.1f,
            groundLayer
        );*/

        //return hit.collider != null
        return isTouchingWall && body.linearVelocityY <= 0.1 && (PlayerData.wallJumpUnlocked || abilityDebug);
    }

    public bool StuckToWallBuffered()
    {
        if (StuckToWall())
            wallRememberTimer = wallRememberTime;
        else
            wallRememberTimer -= Time.deltaTime;
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
            //body.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
            jumpHoldCounter = maxJumpHoldFrames;
            
            
            //starting from midair (double jump)
        } else if (!doubleJumpUsed && (PlayerData.doubleJumpUnlocked || abilityDebug))
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpStrength);
            //body.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
            jumpHoldCounter = maxJumpHoldFrames;
            doubleJumpUsed = true;
        }
    }

    //continue to apply force if jump is held
    private void ApplyJumpHold()
    {
        if (jumpHeld && jumpHoldCounter > 0)
        {
            body.linearVelocity += Vector2.up * (jumpStrength * jumpIncreasePerFrameHeld) * Time.fixedDeltaTime;
            //body.linearVelocity = new Vector2(body.linearVelocity.x, jumpStrength);
            jumpHoldCounter--;
        }
    }

    //immediatly slow down y velocity when jump is released or exceeds maximum height
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
        /*float accel = IsGroundedBuffered() ? accelGrounded : accelInAir;
        float newVelX = Mathf.MoveTowards(body.linearVelocity.x, -horizontalMovement * speed, accel * Time.fixedDeltaTime);*/
        if (PlayerData.wallJumpUnlocked || abilityDebug)
        {

            float wallJumpHorizontalForce = facingRight ? -5.5f : 5.5f;
            body.linearVelocity = new Vector2(wallJumpHorizontalForce, jumpStrength * 1.3f);
        }
        //body.linearVelocity = new Vector2(body.linearVelocityX, jumpStrength * 1.2f);

    }
}