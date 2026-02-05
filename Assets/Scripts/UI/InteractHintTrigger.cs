using TMPro;
using UnityEngine;

public class InteractHintTrigger : MonoBehaviour
{
    private GameObject interactHintObj;
    private TextMeshProUGUI interactTextObj;
    public bool shouldCheckForCollision = true;
    public string interactText = "Interact";

    private void Start()
    {
        interactHintObj = GameObject.FindGameObjectWithTag("InteractHintUI");
        interactHintObj = interactHintObj.transform.GetChild(0).gameObject;
        interactTextObj = interactHintObj.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log("interact hint trigger awake");
        if (interactHintObj == null)
        {
            Debug.Log("couldnt get the interact hint object");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && shouldCheckForCollision && PlayerData.currentHealth < PlayerData.maxHealth)
        {
            SetInteractPopupActive(true);

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
        if (interactHintObj != null)
        {
            interactHintObj.SetActive(active);
            interactTextObj.text = interactText;

        } else
        {
            Debug.Log("Interact hint UI obj not set");
        }
    }
}
