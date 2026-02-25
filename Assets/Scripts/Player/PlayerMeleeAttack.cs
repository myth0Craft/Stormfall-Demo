using Unity.VisualScripting;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    private PlayerControls controls;
    private PlayerMovement playerMovement;
    private bool attackPressed;
    private float attackDurationSeconds = 0.12f;
    private float attackTimer = 0;
    private float attackCooldownDurationSeconds = 0.4f;
    private float attackCooldownTimer = 0;
    //BoxCollider2D playerBox;
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private Animator attackAnimator;
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
        playerMovement.enableSword();
        playerMovement.enableShield();
    }

    private void Update()
    {
        //set the attack hitbox direction
        Vector3 playerPos = playerMovement.transform.position;
        Vector3 offsetVector = playerMovement.getFacingDirection() ? new Vector3(0.5f, 0, 0) : new Vector3(-0.5f, 0, 0);
        attackHitbox.transform.position = playerPos += offsetVector;


        //if the player dashes, disable the attack
        if (playerMovement.getDashFrames() > 0)
        {
            
            attackTimer = 0f;
            attackHitbox.SetActive(false);
            playerMovement.enableSword();

            if (isMidAttack)
            {
                attackPressed = true;
            }
            isMidAttack = false;
        }

        //handle in-between attack cooldown
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer < 0)
                attackCooldownTimer = 0;
        }

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
            if (attackCooldownTimer < 0.03 && attackTimer == 0)
            {
                StartAttack();

            } else
            {
                attackAnimator.SetBool("attackQueued", true);
            }
            if (playerMovement.getDashFrames() <= 0)
            {
                attackPressed = false;
            }
        }

        if (attackPressed && PlayerData.gamePaused)
        {
            attackPressed = false;
        }
    }

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

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }
    
    public void StartAttack()
    {
        if (!PlayerData.gamePaused) {
            if (!isMidAttack)
            {
                if (playerMovement.getDashFrames() <= 0)
                {
                    attackAnimator.SetTrigger("SwingSword");
                    playerMovement.disableSword();
                    playerMovement.bodyDrawSword();
                    isMidAttack = true;
                    
                }
            }
        }
    }

    public void ApplyDamage()
    {
        attackAnimator.SetBool("attackQueued", false);
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
