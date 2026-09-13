using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    public static PlayerAnimationManager instance;

    [SerializeField] private Animator capeAnim;
    [SerializeField] private Animator armsAnim;
    [SerializeField] private Animator bodyAnim;
    [SerializeField] private Animator legsAnim;
    [SerializeField] private Animator swordAnim;
    [SerializeField] private Animator shieldAnim;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        
    }

    private void Start()
    {
        playerMovement = PlayerMovement.instance;

        if (PlayerData.swordUnlocked || PlayerMeleeAttack.instance.attackDebugActive)
        {
            enableSword();
        }

    }


    private void Update()
    {
        capeAnim.SetBool("moving", (playerMovement.body.linearVelocity.x < -3f || playerMovement.body.linearVelocity.x > 3f) && !playerMovement.StuckToWallBuffered());
        bodyAnim.SetBool("moving", (playerMovement.horizontalInput > 0.01f || playerMovement.horizontalInput < -0.01f));
        legsAnim.SetBool("moving", (playerMovement.horizontalInput > 0.01f || playerMovement.horizontalInput < -0.01f));

        if (PlayerData.swordUnlocked || PlayerMeleeAttack.instance.attackDebugActive)
        {
            swordAnim.SetBool("moving", (playerMovement.horizontalInput > 0.01f || playerMovement.horizontalInput < -0.01f) && playerMovement.IsGroundedBuffered());
        }

        if (PlayerData.shieldUnlocked)
        {
            shieldAnim.SetBool("moving", (playerMovement.horizontalInput > 0.01f || playerMovement.horizontalInput < -0.01f) && playerMovement.IsGroundedBuffered());
        }

        capeAnim.SetFloat("jumpTime", playerMovement.jumpTime);
        capeAnim.SetBool("falling", playerMovement.body.linearVelocity.y < -0.1f && !playerMovement.IsGroundedBuffered() && !playerMovement.StuckToWallBuffered());
        capeAnim.SetBool("grounded", playerMovement.IsGroundedBuffered());


        if (PlayerData.wallJumpUnlocked || playerMovement.abilityDebug)
        {
            capeAnim.SetBool("stuckToWall", playerMovement.StuckToWallBuffered());
            legsAnim.SetBool("stuckToWall", playerMovement.StuckToWallBuffered());
            bodyAnim.SetBool("stuckToWall", playerMovement.StuckToWallBuffered());
        }

        //legsAnim.SetFloat("jumpTime", playerMovement.jumpTime);
        legsAnim.SetBool("falling", playerMovement.body.linearVelocity.y < -0.1f && !playerMovement.IsGroundedBuffered() && !playerMovement.StuckToWallBuffered());
        legsAnim.SetBool("grounded", playerMovement.IsGroundedBuffered());


        bodyAnim.SetBool("falling", playerMovement.body.linearVelocity.y < -0.1f && !playerMovement.IsGroundedBuffered() && !playerMovement.StuckToWallBuffered());
        bodyAnim.SetBool("grounded", playerMovement.IsGroundedBuffered());

        if (PlayerData.sprintUnlocked || playerMovement.abilityDebug)
        {
            bodyAnim.SetBool("sprint", playerMovement.currentHorizontalState == HorizontalState.Sprinting && Mathf.Abs(playerMovement.body.linearVelocity.x) > 0.01f);
            legsAnim.SetBool("sprint", playerMovement.currentHorizontalState == HorizontalState.Sprinting && Mathf.Abs(playerMovement.body.linearVelocity.x) > 0.01f);
            armsAnim.SetBool("sprint", playerMovement.currentHorizontalState == HorizontalState.Sprinting && Mathf.Abs(playerMovement.body.linearVelocity.x) > 0.01f);
            capeAnim.SetBool("sprint", playerMovement.currentHorizontalState == HorizontalState.Sprinting && Mathf.Abs(playerMovement.body.linearVelocity.x) > 0.01f);
            swordAnim.SetBool("sprint", playerMovement.currentHorizontalState == HorizontalState.Sprinting && Mathf.Abs(playerMovement.body.linearVelocity.x) > 0.01f);
            shieldAnim.SetBool("sprint", playerMovement.currentHorizontalState == HorizontalState.Sprinting && Mathf.Abs(playerMovement.body.linearVelocity.x) > 0.01f);
        }


        armsAnim.SetBool("moving", (playerMovement.horizontalInput > 0.01f || playerMovement.horizontalInput < -0.01f));


        //at the start of a jump, set jump animation triggers
        if (playerMovement.body.linearVelocity.y > 0.1f && playerMovement.body.linearVelocity.y < 5f && !playerMovement.IsGroundedBuffered() && !playerMovement.StuckToWallBuffered())
        {
            capeAnim.SetTrigger("jump");
            legsAnim.SetTrigger("jump");
            bodyAnim.SetTrigger("jump");
        }

        if (PlayerData.wallJumpUnlocked || playerMovement.abilityDebug)
        {
            bodyAnim.SetBool("WallSliding", playerMovement.currentVerticalState == VerticalState.StuckToWall);
            legsAnim.SetBool("WallSliding", playerMovement.currentVerticalState == VerticalState.StuckToWall);
        }
    }

    public void LungeAttack()
    {
        armsAnim.Play("LungeAttack");
        bodyAnim.SetTrigger("LungeAttack");
        legsAnim.SetTrigger("LungeAttack");
    }

    public void UpwardSlash()
    {
        disableSword();
        armsAnim.Play("UpwardSlash");
    }

    public void DownwardSlash()
    {
        disableSword();
        armsAnim.Play("DownwardSlash");
    }

    public void ShieldBounce() {
        armsAnim.SetTrigger("ShieldBounce");
        bodyAnim.SetTrigger("ShieldBounce");
        legsAnim.SetTrigger("ShieldBounce");
        shieldAnim.SetTrigger("ShieldBounce");
        capeAnim.SetTrigger("ShieldBounce");

    }

    public void enableSword()
    {
        swordAnim.SetBool("hasSword", true);
    }

    public void disableSword()
    {
        swordAnim.SetBool("hasSword", false);
    }

    public void enableShield()
    {
        shieldAnim.SetBool("hasShield", true);
    }

    public void disableShield()
    {
        shieldAnim.SetBool("hasShield", false);
    }

    public void SetCapeJumpTrigger()
    {
        capeAnim.SetTrigger("jump");
    }

    public void Dash()
    {
        capeAnim.SetTrigger("dash");
        armsAnim.SetTrigger("dash");
        bodyAnim.SetTrigger("dash");
        legsAnim.SetTrigger("dash");
    }

    public void SetTurnTrigger()
    {
        bodyAnim.SetTrigger("turn");
    }

    public void SetAttackQueued(bool queued)
    {
        armsAnim.SetBool("attackQueued", queued);
    }

    public void SetSwingSwordTrigger()
    {
        armsAnim.SetTrigger("SwingSword");
        bodyAnim.SetTrigger("drawSword");
    }

    public void SetComboAttackTrigger()
    {
        armsAnim.SetTrigger("ComboAttack");
    }

    public void SetGainSwordAbility(bool gainSwordSequenceActive)
    {
        armsAnim.SetBool("gainSwordAbility", gainSwordSequenceActive);
    }

    public void StartOverheadSlash()
    {
        bodyAnim.SetTrigger("OverheadSlash");
        capeAnim.SetTrigger("OverheadSlash");
    }

    public void Block()
    {
        armsAnim.Play("Block");
    }

    public void SetShieldSlide(bool isShieldSliding)
    {
        bodyAnim.SetBool("ShieldSliding", isShieldSliding);
        armsAnim.SetBool("ShieldSliding", isShieldSliding);
        capeAnim.SetBool("ShieldSliding", isShieldSliding);
        swordAnim.SetBool("ShieldSliding", isShieldSliding);
        shieldAnim.SetBool("ShieldSliding", isShieldSliding);
        legsAnim.SetBool("ShieldSliding", isShieldSliding);
    }

    public void PlaySwordSheath()
    {
        armsAnim.Play("SheathSword");
    }

    public void PlaySwordDraw()
    {
        armsAnim.Play("DrawSword");
    }

    public void PlayBasicAttack(int comboNum)
    {
        disableSword();
        if (comboNum > 2)
        {
            Debug.Log("Invalid combo number!");
            return;
        }
        armsAnim.Play("BasicAttack" + (comboNum + 1));
        //armsAnim.Play("DownwardSlash");
    }

    public void Jump()
    {
        legsAnim.Play("Jump");
        armsAnim.Play("Jump");
        bodyAnim.Play("Jump");
    }

    public void Land()
    {
        legsAnim.Play("Land");
        armsAnim.Play("Land");
        bodyAnim.Play("Land");
    }
}