using UnityEngine;

public class ConinuousDamageObject : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthManager health = other.GetComponent<PlayerHealthManager>();
            health.ApplyDamage();
        }
    }
}
