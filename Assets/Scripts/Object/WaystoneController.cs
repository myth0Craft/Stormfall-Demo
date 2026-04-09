using System.Collections;
using TMPro;
using UnityEngine;

public class WaystoneController : MonoBehaviour
{
    [SerializeField] private GameObject inactive;
    [SerializeField] private GameObject active;
    [SerializeField] private string id;

    [SerializeField] private bool waystoneActive = true;

    private InteractHintTrigger interactHintTrigger;

    private bool interactPressed;
    private PlayerControls controls;

    private TextMeshProUGUI text;

    private void Awake()
    {

        text = GetComponentInChildren<TextMeshProUGUI>();
        text.gameObject.SetActive(false);

        controls = PlayerData.getControls();
        interactHintTrigger = GetComponent<InteractHintTrigger>();
        controls.Player.Interact.performed += ctx => interactPressed = true;

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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && waystoneActive)
        {
            if (interactPressed)
            {
                interactPressed = false;
                Deactivate();
            }

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactPressed = false;
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
