using System.Collections;
using UnityEngine;

public class SavePointHealthRestore : MonoBehaviour
{
    private InteractHintTrigger interactHintTrigger;
    private bool interactPressed;
    private PlayerControls controls;
    private bool isCurrentlyRestoringHealth = false;

    private PlayerHealthManager playerHealth;

    private void Awake()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>();
        controls = PlayerData.getControls();
        interactHintTrigger = GetComponent<InteractHintTrigger>();
        controls.Player.Interact.performed += ctx => interactPressed = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactPressed = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {   
        if (collision.CompareTag("Player"))
        {
            if (interactPressed)
            {
                
                interactPressed = false;
                if (playerHealth.currentHealth < playerHealth.getMaxHealth() && !isCurrentlyRestoringHealth)
                {
                    StartCoroutine(RestoreHealthCoroutine());
                    isCurrentlyRestoringHealth = true;
                }
                
            } else if (!isCurrentlyRestoringHealth && playerHealth.currentHealth < playerHealth.getMaxHealth())
            {
                interactHintTrigger.SetInteractPopupActive(true);
            }
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
