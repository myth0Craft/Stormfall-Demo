using UnityEngine;

public class CorruptedCrawler : MonoBehaviour
{

    [SerializeField] private bool facingRight = true;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float accel = 2f;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private LayerMask groundLayer;
    private LayerMask enemyLayer;

    private void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
        enemyLayer = LayerMask.GetMask("Enemy");
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        float targetSpeed = facingRight ? speed : -speed;

        bool facingPlayer = false;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, 10f);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                facingPlayer = true;
            }
        }


        float newVelX = Mathf.MoveTowards(
            body.linearVelocity.x,
            facingPlayer ? targetSpeed * 2 : targetSpeed,
            accel * Time.fixedDeltaTime
        );

        body.linearVelocity = new Vector2(newVelX, body.linearVelocity.y);

        Vector2 origin = new Vector2(
            facingRight ? boxCollider.bounds.max.x : boxCollider.bounds.min.x,
            boxCollider.bounds.center.y
        );

        Vector2 ledgeDetectOrigin = new Vector2(
            facingRight ? boxCollider.bounds.max.x + 0.5f : boxCollider.bounds.min.x - 0.5f,
            boxCollider.bounds.min.y
        );

        Vector2 direction = facingRight ? Vector2.right : Vector2.left;

        if (Physics2D.Raycast(origin, direction, 0.1f, groundLayer))
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

