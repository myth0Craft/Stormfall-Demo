using TMPro;
using UnityEngine;

public class InteractHintTrigger : MonoBehaviour
{
    //private GameObject interactHintObj;
    //private TextMeshProUGUI interactTextObj;
    public bool shouldCheckForCollision = true;
    public string interactText = "Interact";
    public bool onlyCheckIfHealthLow = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && shouldCheckForCollision)
        {
            
                if (onlyCheckIfHealthLow)
                {
                    if (PlayerData.currentHealth < PlayerData.maxHealth)
                    {
                        SetInteractPopupActive(true);
                    }
                }
                else
                {
                    SetInteractPopupActive(true);
                }
            
            

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetInteractPopupActive(false);
        }
    }

    public void SetInteractPopupActive(bool active)
    {
        if (active)
        {
            InteractUIController.Instance.enableInteractUI(this.interactText);
        }
        else
        {
            InteractUIController.Instance.disableInteractUI();
        }

    }
    
}
