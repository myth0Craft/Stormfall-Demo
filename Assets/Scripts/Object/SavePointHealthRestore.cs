using System.Collections;
using UnityEngine;

public class SavePointHealthRestore : MonoBehaviour, IInteractable
{
    private InteractHintTrigger interactHintTrigger;
    private bool isCurrentlyRestoringHealth = false;

    private PlayerHealthManager playerHealth;

    public AudioClip healSound;

    private void Awake()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>();
        interactHintTrigger = GetComponent<InteractHintTrigger>();
    }

    public void Interact()
    {
        if (playerHealth.currentHealth < playerHealth.getMaxHealth() && !isCurrentlyRestoringHealth)
        {
            AudioSource.PlayClipAtPoint(healSound, transform.position, 0.25f);
            StartCoroutine(RestoreHealthCoroutine());
            isCurrentlyRestoringHealth = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isCurrentlyRestoringHealth && playerHealth.currentHealth < playerHealth.getMaxHealth())
            {
                interactHintTrigger.SetInteractPopupActive(true);
                PlayerContextActionInputManager.instance.SetInteractable(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if(collision.CompareTag("Player"))
        {
            PlayerContextActionInputManager.instance.ClearInteractable(this);
        }
        
    }


    private IEnumerator RestoreHealthCoroutine()
    {
        interactHintTrigger.SetInteractPopupActive(false);
        interactHintTrigger.shouldCheckForCollision = false;
        while(playerHealth.currentHealth < playerHealth.getMaxHealth())
        {
            playerHealth.Heal(1);
            yield return new WaitForSecondsRealtime(0.3f);
        }
        interactHintTrigger.shouldCheckForCollision = true;
        isCurrentlyRestoringHealth = false;
    }
}
