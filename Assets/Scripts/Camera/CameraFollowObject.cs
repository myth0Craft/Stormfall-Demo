using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{

    [SerializeField] private Transform playerTransform;
    private PlayerMovement player;
    private float flipOffset = 0.7f;
    private float currentOffset;
    private float smoothSpeed = 3f;
    private float minMoveTime = 0.1f;
    private float moveTimer = 0f;
    private float lastActiveOffset = 0f;
    float lastDir = 0f;
    private bool facingRight = true;
    private float currentY;
    private float verticalDeadZone = 0.5f;
    private Vector3 velocity = Vector3.zero;
    private float verticalSmoothTime = 0.2f;
    private float yBeforeJump;
    private float oldYBeforeJump;


    //private bool facingRight;
    //private bool checkGrounded = false;

    private void Awake()
    {
        player = playerTransform.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        //stores the ground y if player is grounded
        /*if (player.IsGrounded() && player.getLinearVelocity().y == 0)
        {
            //oldYBeforeJump = yBeforeJump;
            yBeforeJump = playerTransform.position.y;
            *//*if (Mathf.Abs(oldYBeforeJump - yBeforeJump) <= 0.1)
            {
                yBeforeJump = oldYBeforeJump;
            }*//*
        }*/

        Vector3 targetOffset = new Vector3(getXOffset(), 0, 0);

        //add calculated offsets to player's pos to build the final camera pos
        Vector3 targetPosition = playerTransform.position + targetOffset;
        /*if (!playerInVerticalDeadZone())
        {
            if (shouldLerpY())
            {
                targetPosition.y = Mathf.Lerp(transform.position.y, playerTransform.position.y, Time.deltaTime * smoothSpeed);
            }

        }
        else
        {
            targetPosition.y = Mathf.Lerp(transform.position.y, yBeforeJump, Time.deltaTime * smoothSpeed);
        }
        if (!shouldLerpY())
        {
            if (transform.position.y < playerTransform.position.y)
                targetPosition.y = Mathf.Lerp(transform.position.y, playerTransform.position.y, Time.deltaTime * smoothSpeed);
        }*/
        /*if (!player.IsGroundedBuffered())
        {
            if (!player.isWallJumping)
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, targetPosition.x, Time.deltaTime * smoothSpeed), targetPosition.y, targetPosition.z);
            else
               transform.position = new Vector3(transform.position.x, targetPosition.y, targetPosition.z);
        } else
        {
            transform.position = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
        }*/

        transform.position = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
    }

    public bool shouldLerpY()
    {
        if (player.getLinearVelocity().y < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool playerInVerticalDeadZone()
    {
        if (playerTransform.position.y - yBeforeJump > verticalDeadZone || playerTransform.position.y - yBeforeJump < -verticalDeadZone)
            return false;
        else
            return true;
    }

    public float getXOffset()
    {


        float targetOffset = player.getFacingDirection() ? flipOffset : -flipOffset;

        float xVel = player.getLinearVelocity().x;

        if (Mathf.Abs(xVel) > 0.2f)
        {
            moveTimer += Time.deltaTime;

            if (moveTimer > minMoveTime)
            {
                targetOffset = Mathf.Clamp(xVel, -flipOffset, flipOffset);

            }
        }
        else
        {
            moveTimer = 0;
        }

        currentOffset = Mathf.Lerp(currentOffset, targetOffset, Time.deltaTime * smoothSpeed);

        return currentOffset;
    }
}
