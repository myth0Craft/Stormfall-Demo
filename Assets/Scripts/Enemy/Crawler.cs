using UnityEngine;

public class Crawler : MonoBehaviour
{

    [SerializeField] private bool facingRight = true;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float accel = 2f;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private LayerMask groundLayer;

    private void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        Vector2 groundCheckOrigin = new Vector2(
            boxCollider.bounds.center.x, boxCollider.bounds.min.y);
        //ground detection
        if (!Physics2D.BoxCast(groundCheckOrigin, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer))
        {
            return;
        }

        /*if (!Physics2D.Raycast(groundCheckOrigin, Vector2.down, 0.1f, groundLayer))
        {
            return;
        }*/


        float targetSpeed = facingRight ? speed : -speed;

        float newVelX = Mathf.MoveTowards(
            body.linearVelocity.x,
            targetSpeed,
            accel * Time.fixedDeltaTime
        );

        body.linearVelocity = new Vector2(newVelX, body.linearVelocity.y);

        Vector2 wallCheckOrigin = new Vector2(
            facingRight ? boxCollider.bounds.max.x : boxCollider.bounds.min.x,
            boxCollider.bounds.center.y
        );

        Vector2 ledgeDetectOrigin = new Vector2(
            facingRight ? boxCollider.bounds.max.x + 0.5f : boxCollider.bounds.min.x - 0.5f,
            boxCollider.bounds.min.y
        );

        Vector2 direction = facingRight ? Vector2.right : Vector2.left;

        //wall detection
        if (Physics2D.Raycast(wallCheckOrigin, direction, 0.1f, groundLayer))
            //|| Physics2D.Raycast(origin, direction, 0.1f, enemyLayer))
        {
            TriggerTurn();
        }

        //ledge detection
        if (!Physics2D.Raycast(ledgeDetectOrigin, Vector2.down, 0.1f, groundLayer))
        {
            TriggerTurn();
        }
    }

    private void TriggerTurn()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}

