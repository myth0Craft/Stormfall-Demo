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

        Vector3 targetOffset = new Vector3(getXOffset(), 0, 0);

        
        Vector3 targetPosition = playerTransform.position + targetOffset;

        transform.position = new Vector3(targetPosition.x, Mathf.Lerp(transform.position.y, targetPosition.y, Time.deltaTime * smoothSpeed), targetPosition.z);
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
