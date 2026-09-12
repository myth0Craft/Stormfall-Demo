using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CombatState
{
    Idle,
    Active,
    Cooldown,
    Drawing,
    Blocking
}

public enum AttackType
{
    None,
    Basic,
    Upward,
    Downward,
    Lunge
}

public class PlayerMeleeAttack : MonoBehaviour
{

    public CombatState currentCombatState { get; private set; }

    public bool swordDrawn = false;
    private AttackType pendingAttack = AttackType.None;

    private Coroutine hitboxCoroutine;

    public static PlayerMeleeAttack instance;
    private PlayerControls controls;
    private PlayerMovement playerMovement;
    private float attackHitboxActiveDurationSeconds = 0.12f;
    public bool attackHitboxActive { get; private set; } = false;

    [SerializeField] private BoxCollider2D basicAttackHitbox;
    [SerializeField] private BoxCollider2D upwardAttackHitbox;
    [SerializeField] private BoxCollider2D downwardAttackHitbox;

    [SerializeField] private bool attackDebug;
    public bool attackDebugActive { get; private set; } = false;

    public int comboNum = 0;

    public int combatStaminaCost = 15;
    public int dashAttackStaminaCost = 30;
    public float blockStaminaCost = 20;
    public float staminaRequiredToBlock = 10;
    public float amountOfStaminaRestoredOnParry = 20;

    public GameObject blockParticle;

    private bool successfullyParried = false;

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
        controls = PlayerData.getControls();
        controls.Player.Attack.performed += OnAttackPressed;

        disableAllHitboxes();

        attackDebugActive = attackDebug;

        currentCombatState = CombatState.Idle;

        OnBetaFeaturesToggled();

    }
    public void ResetCombatState()
    {
        currentCombatState = CombatState.Idle;
        comboNum = 0;

        disableAllHitboxes();

        attackHitboxActive = false;
        PlayerAnimationManager.instance.enableSword();
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

        BetaFeaturesToggleButton.onBetaFeaturesToggled += OnBetaFeaturesToggled;
    }

    void OnDisable()
    {
        if (controls != null)
        {
            controls.Player.Disable();
        }
        controls.Player.Attack.performed -= OnAttackPressed;

        BetaFeaturesToggleButton.onBetaFeaturesToggled -= OnBetaFeaturesToggled;
    }

    public void OnBlock()
    {
        if (!(currentCombatState == CombatState.Idle || currentCombatState == CombatState.Cooldown)) return;

        //if (!StaminaManager.instance.CanAffordStaminaCost(staminaRequiredToBlock)) return;


        if (currentCombatState == CombatState.Cooldown)
        {
            ResetCombatState();
        }

        successfullyParried = false;
        PlayerAnimationManager.instance.Block();
        currentCombatState = CombatState.Blocking;
        StartCoroutine(BlockCoroutine());
    }

    private IEnumerator BlockCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        currentCombatState = CombatState.Idle;
        if (!successfullyParried)
        {
            StaminaManager.instance.DecrementStamina(blockStaminaCost);
        }
        successfullyParried = false;
        ResetCombatState();
    }

    public void AddBlockEffects()
    {
        successfullyParried = true;
        StaminaManager.instance.RestoreStamina(amountOfStaminaRestoredOnParry);
        Instantiate(blockParticle, transform.position, Quaternion.identity);
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        GlobalHitstopManager.DoHitStopThenHitSlow(0.05f, 0.15f, 0.25f);
        CamShakeSource.instance.AddScreenShake(0.2f);

        ShockwaveEffectManager.instance.SetSpeed(3f);
        ShockwaveEffectManager.instance.StartShockwave(new Vector2(viewportPos.x, viewportPos.y));
    }

    private void OnAttackPressed(InputAction.CallbackContext context)
    { 
        if ((PlayerData.swordUnlocked || attackDebugActive) && !PlayerData.gamePaused)
        {
            if (!StaminaManager.instance.CanAffordStaminaCost(combatStaminaCost))
            {
                return;
            }

            if (!(currentCombatState == CombatState.Idle || currentCombatState == CombatState.Cooldown)) return;


            AttackType requestedAttack;

            if (playerMovement.currentHorizontalState == HorizontalState.Dashing && playerMovement.IsGroundedBuffered())
            {
                if (!StaminaManager.instance.CanAffordStaminaCost(dashAttackStaminaCost)) return;

                requestedAttack = AttackType.Lunge;

            } else if (controls.Player.Move.ReadValue<Vector2>().y > 0.1f)
            {
                requestedAttack = AttackType.Upward;

            } else if (controls.Player.Move.ReadValue<Vector2>().y < -0.1f)
            {
                requestedAttack = AttackType.Downward;
            } else
            {
                requestedAttack = AttackType.Basic;
            }

            if (!swordDrawn)
            {
                pendingAttack = requestedAttack;
                currentCombatState = CombatState.Drawing;
                PlayerAnimationManager.instance.PlaySwordDraw();
                return;
            }

            ExecuteAttack(requestedAttack);
        }
    }

    private void ExecuteAttack(AttackType requestedAttack)
    {
        pendingAttack = AttackType.None;


        switch (requestedAttack)
        {
            case AttackType.Basic:
                PerformBasicAttack();
                break;

            case AttackType.Upward:
                PerformUpwardSlash();
                break;

            case AttackType.Downward:
                PerformDownwardSlash();
                break;

            case AttackType.Lunge:
                PerformDashAttack();
                break;
        }
    }

    public void SwordDrawFinished()
    {
        swordDrawn = true;

        if (pendingAttack != AttackType.None)
        {
            AttackType attack = pendingAttack;
            pendingAttack = AttackType.None;


            ExecuteAttack(attack);
        }
        else
        {
            currentCombatState = CombatState.Idle;
        }
    }

    public void SetCombatState(CombatState combatState)
    {
        this.currentCombatState = combatState;
    }

    public void PerformBasicAttack()
    {
        StaminaManager.instance.DecrementStamina(combatStaminaCost);

        if (currentCombatState == CombatState.Idle)
        {
            currentCombatState = CombatState.Active;
            comboNum = 0;
        }
        else if (currentCombatState == CombatState.Cooldown)
        {
            currentCombatState = CombatState.Active;

            comboNum++;

            if (comboNum > 2)
            {
                comboNum = 0;
            }
        }
        PlayerAnimationManager.instance.PlayBasicAttack(comboNum);
    }

    private void PerformUpwardSlash()
    {
        comboNum = 0;
        currentCombatState = CombatState.Active;
        StaminaManager.instance.DecrementStamina(combatStaminaCost);
        PlayerAnimationManager.instance.UpwardSlash();
    }

    private void PerformDownwardSlash()
    {
        comboNum = 0;
        currentCombatState = CombatState.Active;
        StaminaManager.instance.DecrementStamina(combatStaminaCost);
        PlayerAnimationManager.instance.DownwardSlash();
    }

    private void PerformDashAttack()
    {
        currentCombatState = CombatState.Active;
        StaminaManager.instance.DecrementStamina(dashAttackStaminaCost);
        PlayerAnimationManager.instance.LungeAttack();
        PlayerAnimationManager.instance.disableSword();
        PlayerMovement.instance.currentHorizontalState = HorizontalState.Dashing;
        PlayerMovement.instance.OnDashAttack();
    }

    //updates attack damage hitbox position to be in front of the player
    private void UpdateFacingDirection()
    {
        Vector3 playerPos = playerMovement.transform.position;

        float multiplier;

        if (playerMovement.currentVerticalState == VerticalState.StuckToWall)
        {
            multiplier = -1f;
        } else
        {
            multiplier = 1f;
        }

        Vector3 offsetMultiplier = new Vector3((playerMovement.getFacingDirection() ? 1 : -1) * multiplier, 1, 1);

        //Vector3 offsetVector = playerMovement.getFacingDirection() ? new Vector3(0.5f * multiplier, 0, 0) : new Vector3(-0.5f * multiplier, 0, 0);
        basicAttackHitbox.gameObject.transform.parent.gameObject.transform.localScale = offsetMultiplier;
    }

    public void ApplyDashAttackDamage()
    {
        UpdateFacingDirection();
        basicAttackHitbox.gameObject.SetActive(true);
        PlayerHealthManager.instance.ShouldApplyDamage(false);
    }

    public void ExitDashAttack()
    {
        basicAttackHitbox.gameObject.SetActive(false);
        attackHitboxActive = false;
        PlayerHealthManager.instance.ShouldApplyDamage(true);
    }

    private void disableAllHitboxes()
    {
        basicAttackHitbox.gameObject.SetActive(false);
        upwardAttackHitbox.gameObject.SetActive(false);
        downwardAttackHitbox.gameObject.SetActive(false);
    }

    public IEnumerator HitboxCoroutine(BoxCollider2D hitbox)
    {
        UpdateFacingDirection();
        hitbox.gameObject.SetActive(true);

        yield return new WaitForSeconds(attackHitboxActiveDurationSeconds);

        hitbox.gameObject.SetActive(false);
    }

    public void ActivateBasicAttackHitbox()
    {
        disableAllHitboxes();
        if (hitboxCoroutine != null)
        {
            StopCoroutine(hitboxCoroutine);
        }
        hitboxCoroutine = StartCoroutine(HitboxCoroutine(basicAttackHitbox));
    }

    public void ActivateUpwardSlashHitbox()
    {
        disableAllHitboxes();
        if (hitboxCoroutine != null)
        {
            StopCoroutine(hitboxCoroutine);
        }
        hitboxCoroutine = StartCoroutine(HitboxCoroutine(upwardAttackHitbox));
    }

    public void ActivateDownwardSlashHitbox()
    {
        disableAllHitboxes();
        if (hitboxCoroutine != null)
        {
            StopCoroutine(hitboxCoroutine);
        }
        hitboxCoroutine = StartCoroutine(HitboxCoroutine(downwardAttackHitbox));
    }

    private void OnBetaFeaturesToggled()
    {

        if (!Application.isEditor)
        {
            attackDebug = PlayerData.betaFeaturesEnabled;
            attackDebugActive = PlayerData.betaFeaturesEnabled;
        }

    }
}