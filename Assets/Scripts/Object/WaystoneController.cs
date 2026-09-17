using System.Collections;
using TMPro;
using UnityEngine;

public class WaystoneController : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject inactive;
    [SerializeField] private GameObject active;
    [SerializeField] private string id;

    [SerializeField] private bool waystoneActive = true;

    private InteractHintTrigger interactHintTrigger;


    private TextMeshProUGUI text;

    private void Awake()
    {

        text = GetComponentInChildren<TextMeshProUGUI>();
        text.gameObject.SetActive(false);
        interactHintTrigger = GetComponent<InteractHintTrigger>();

        inactive.SetActive(false);
        active.SetActive(true);

        if (id == null)
        {
            Debug.Log("Id of Waystone is null!");
        }
        else
        {
            var room = SaveSystem.getRoom(gameObject.scene.name);

            if (room.pickups.TryGetValue(id, out bool collected) && collected)
            {
                Debug.Log("waystone collected previously");
                waystoneActive = false;
                inactive.SetActive(true);
                active.SetActive(false);
                interactHintTrigger.SetInteractPopupActive(false);
                interactHintTrigger.shouldCheckForCollision = false;
            }
        }

        //if (waystoneActive)
        //{
        //    inactive.SetActive(false);
        //    active.SetActive(true);
        //} else
        //{
        //    inactive.SetActive(true);
        //    active.SetActive(false);
        //}
    }

    private void OnDestroy()
    {
        //interactHintTrigger.SetInteractPopupActive(false);
    }

    public void Interact()
    {
        Deactivate();

        if (id == null)
        {
            Debug.Log("Id of Waystone is null!");
        }
        else
        {
            var room = SaveSystem.getRoom(gameObject.scene.name);
            room.pickups[id] = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && waystoneActive)
        {
            interactHintTrigger.SetInteractPopupActive(true);
            PlayerContextActionInputManager.instance.SetInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactHintTrigger.SetInteractPopupActive(false);
            PlayerContextActionInputManager.instance.ClearInteractable(this);
        }
    }

    private void Deactivate()
    {
        

        interactHintTrigger.SetInteractPopupActive(false);
        interactHintTrigger.shouldCheckForCollision = false;
        inactive.SetActive(true);
        active.SetActive(false);
        StartCoroutine(DemoCompleteTextFadeIn());
    }

    private IEnumerator DemoCompleteTextFadeIn()
    {
        text.color = new Color(255, 255, 255, 0);
        text.gameObject.SetActive(true);

        float demoCompleteFadeInTime = 0f;
        while (demoCompleteFadeInTime < 1f)
        {
            demoCompleteFadeInTime += Time.unscaledDeltaTime;
            float time = Mathf.Clamp01(demoCompleteFadeInTime / 1.0f);
            text.color = Color.Lerp(new Color(text.color.r, text.color.g, text.color.b, 0), new Color(text.color.r, text.color.g, text.color.b, 1), time);
            yield return null;
        }

        yield return new WaitForSeconds(3.0f);

        demoCompleteFadeInTime = 0f;

        while (demoCompleteFadeInTime < 1f)
        {
            demoCompleteFadeInTime += Time.unscaledDeltaTime;
            float time = Mathf.Clamp01(demoCompleteFadeInTime / 1.0f);
            text.color = Color.Lerp(new Color(text.color.r, text.color.g, text.color.b, 1), new Color(text.color.r, text.color.g, text.color.b, 0), time);
            yield return null;
        }
        text.gameObject.SetActive(false);
    }
}
