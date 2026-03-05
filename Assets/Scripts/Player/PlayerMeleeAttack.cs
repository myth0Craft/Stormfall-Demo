using Unity.VisualScripting;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    private PlayerControls controls;
    private PlayerMovement playerMovement;
    private bool attackPressed;
    private float attackDurationSeconds = 0.12f;
    private float attackTimer = 0;

    public float getAttackTimerTime()
    {
        return attackTimer;
    }

    private float attackCooldownDurationSeconds = 0.4f;
    public float attackCooldownTimer = 0;
    //BoxCollider2D playerBox;
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private bool attackDebug = false;
    private BoxCollider2D attackCollider;
    private bool oldFacingRight;
    public bool isMidAttack = false;

    public int comboNum = 0;

    private void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        //playerBox = GetComponentInParent<BoxCollider2D>();
        attackCollider = attackHitbox.GetComponent<BoxCollider2D>();
        controls = PlayerData.getControls();
        controls.Player.Attack.performed += ctx => attackPressed = true;
        oldFacingRight = playerMovement.getFacingDirection();
        attackHitbox.SetActive(false);
        if (PlayerData.swordUnlocked)
        {
            PlayerAnimationManager.instance.enableSword();
            //playerMovement.enableShield();
        }
        
    }

    private void Update()
    {
        if (PlayerData.swordUnlocked || attackDebug)
        {


            //if the player dashes, disable the attack
            if (playerMovement.getDashFrames() > 0)
            {
                PlayerAnimationManager.instance.SetAttackQueued(false);
                attackTimer = 0f;
                attackHitbox.SetActive(false);
                PlayerAnimationManager.instance.enableSword();
                //if (isMidAttack)
                //{
                //    attackPressed = true;
                //}
                attackPressed = false;
                isMidAttack = false;
            }

            //handle in-between attack cooldown
            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
                if (attackCooldownTimer < 0)
                    attackCooldownTimer = 0;
            }

            //counts down damage hitbox active time. When it gets disabled, start the in-between attack cooldown timer.
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer < 0)
                    attackTimer = 0;

                if (attackTimer == 0)
                {
                    attackHitbox.SetActive(false);
                    attackCooldownTimer = attackCooldownDurationSeconds;
                    //playerMovement.enableSword();
                }
            }


            if (attackPressed)
            {
                //disable input if game is paused
                if (PlayerData.gamePaused)
                {
                    attackPressed = false;
                }

                //if there is no cooldown active start attack animation
                if (attackCooldownTimer == 0 && attackTimer == 0)
                {
                    StartAttack();


                }
                else if (!isMidAttack)
                {

                    //if pressed mid attack queue a combo attack
                    PlayerAnimationManager.instance.SetAttackQueued(true);
                }

                //disable input if player is dashing
                if (playerMovement.getDashFrames() <= 0)
                {
                    attackPressed = false;
                }
            }
        } else
        {
            attackPressed = false;
        }
    }

    //currently unused
    private void UpdateComboNum()
    {
        if (attackCooldownTimer == 0 && attackTimer == 0)
        {
            comboNum = 0;
        } else { 
            comboNum++;
        if (comboNum >= 2)
            comboNum = 0;
        }
    }

    //updates attack damage hitbox position to be in front of the player
    private void UpdateFacingDirection()
    {
        Vector3 playerPos = playerMovement.transform.position;
        Vector3 offsetVector = playerMovement.getFacingDirection() ? new Vector3(0.5f, 0, 0) : new Vector3(-0.5f, 0, 0);
        attackHitbox.transform.position = playerPos += offsetVector;
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }
    
    //called when the attack animation starts, begins execution of attack anim
    public void StartAttack()
    {
        if (!PlayerData.gamePaused) {
            if (!isMidAttack)
            {
                if (playerMovement.getDashFrames() <= 0)
                {
                    PlayerAnimationManager.instance.disableSword();
                    PlayerAnimationManager.instance.SetSwingSwordTrigger();
                    isMidAttack = true;
                }
            }
        }
    }

    //called from within the animation itself on prespecified "contact frames". Enables damage hitbox.
    public void ApplyDamage()
    {
        UpdateFacingDirection();
        //attackAnimator.SetBool("attackQueued", false);
        attackTimer = attackDurationSeconds;
        attackHitbox.SetActive(true);
        isMidAttack = false;
        comboNum++;
        if (comboNum >= 1)
        {
            comboNum = 0;
        }
    }
}
