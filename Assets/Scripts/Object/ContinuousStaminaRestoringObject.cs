using UnityEngine;

public class ContinuousStaminaRestoringObj : MonoBehaviour
{
    public int amount = 1;
    public float frequency = 0.1f;
    public bool restoringEnabled = true;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && enabled)
        {
            StaminaManager.instance.RestoreStamina(amount * Time.deltaTime * frequency);
        }
    }

}
