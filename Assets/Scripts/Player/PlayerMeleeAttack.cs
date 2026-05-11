using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMeleeAttack : MonoBehaviour
{

    public static PlayerMeleeAttack instance;
    private PlayerControls controls;
    private PlayerMovement playerMovement;
    private bool attackPressed;
    private float attackHitboxActiveDurationSeconds = 0.12f;
    public bool attackHitboxActive { get; private set; } = false;

    private float attackCooldownDurationSeconds = 0.4f;
    private bool attackIsOnCooldown = false;
    [SerializeField] private GameObject attackHitbox;

    [SerializeField] private bool attackDebug;
    public bool attackDebugActive { get; private set; } = false;
    private BoxCollider2D attackCollider;
    public bool isMidAttack = false;

    public int comboNum = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }


        playerMovement = GetComponentInParent<PlayerMovement>();
        attackCollider = attackHitbox.GetComponent<BoxCollider2D>();
        controls = PlayerData.getControls();
        controls.Player.Attack.performed += OnAttackPressed;
        attackHitbox.SetActive(false);
        attackDebugActive = attackDebug;
    }

    

    

    private void OnDestroy()
    {
        controls.Player.Attack.performed -= OnAttackPressed;
    }

    void OnEnable()
    {
        if (controls != null)
        {
            controls.Player.Enable();
        }
        controls.Player.Attack.performed += OnAttackPressed;
    }

    void OnDisable()
    {
        if (controls != null)
        {
            controls.Player.Disable();
        }
        controls.Player.Attack.performed -= OnAttackPressed;
    }

    private void OnAttackPressed(InputAction.CallbackContext context)
    {
        if ((PlayerData.swordUnlocked || attackDebugActive) && !PlayerData.gamePaused && PlayerMovement.instance.currentCombatState != CombatState.Blocking)
        {
            //cancel attack if player is dashing
            /*if (playerMovement.getDashFrames() <= 0)
            {
                Debug.Log("Dash canceled attack");
                return;
            }*/


            if (!attackHitboxActive && !attackIsOnCooldown)
            {
                Debug.Log("Attack started");
                StartAttack();
            }
            else if (!isMidAttack)
            {
                Debug.Log("combo queued");
                //if pressed mid attack queue a combo attack
                PlayerAnimationManager.instance.SetAttackQueued(true);
            }
        } else
        {
            Debug.Log("Cannot attack");
        }


    }

    private IEnumerator AttackCooldownCoroutine()
    {
        attackHitboxActive = true;
        yield return new WaitForSeconds(attackHitboxActiveDurationSeconds);
        attackHitbox.SetActive(false);
        attackHitboxActive = false;

        attackIsOnCooldown = true;
        yield return new WaitForSeconds(attackCooldownDurationSeconds);
        attackIsOnCooldown = false;
    }

    public void CancelAttack()
    {
        PlayerAnimationManager.instance.SetAttackQueued(false);
        StopAllCoroutines();
        attackHitboxActive = false;
        attackIsOnCooldown = false;
        attackHitbox.SetActive(false);
        PlayerAnimationManager.instance.enableSword();
        attackPressed = false;
        isMidAttack = false;
    }

    //currently unused
    //private void UpdateComboNum()
    //{
    //    if (!attackIsOnCooldown && !attackHitboxActive)
    //    {
    //        comboNum = 0;
    //    } else { 
    //        comboNum++;
    //    if (comboNum >= 2)
    //        comboNum = 0;
    //    }
    //}

    //updates attack damage hitbox position to be in front of the player
    private void UpdateFacingDirection()
    {
        Vector3 playerPos = playerMovement.transform.position;
        Vector3 offsetVector = playerMovement.getFacingDirection() ? new Vector3(0.5f, 0, 0) : new Vector3(-0.5f, 0, 0);
        attackHitbox.transform.position = playerPos += offsetVector;
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
        
        attackHitbox.SetActive(true);
        isMidAttack = false;

        StartCoroutine(AttackCooldownCoroutine());

        //comboNum++;
        //if (comboNum >= 1)
        //{
        //    comboNum = 0;
        //}
    }
}
