using UnityEngine;

public class WindGust : MonoBehaviour
{
    public Vector2 playerVelocityModifier = new Vector2(0, 0);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement.instance.AddVelocityModifier(playerVelocityModifier);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement.instance.RemoveVelocityModifier(playerVelocityModifier);
        }
    }
}
