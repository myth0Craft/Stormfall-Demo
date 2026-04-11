using TMPro;
using UnityEngine;

public class InteractUIController : MonoBehaviour
{
    public static InteractUIController Instance { get; private set; }

    public string interactText = "Interact";
    private TextMeshProUGUI textUIElement;

    private void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }

        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }
        textUIElement = GetComponentInChildren<TextMeshProUGUI>();
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    public void enableInteractUI(string interactText)
    {
        this.interactText = interactText;
        
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }
        textUIElement.text = this.interactText;
    }

    public void disableInteractUI()
    {
        if (gameObject != null)
        {
            try
            {

                foreach (Transform t in transform)
                {
                    t.gameObject.SetActive(false);
                }
            } catch
            {
                Debug.Log("An error occured with the interaction UI.");
            }
        }
    }

    public void enableInteractUI()
    {
        enableInteractUI("Interact");
    }
}
