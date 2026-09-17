using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShieldCollectEvent : QuicktimeEvent, IInteractable
{

    private InteractHintTrigger interactHintTrigger;
    private bool used = false;
    private ArenaBattleTrigger arenaBattle;

    private DisplaySaveIcon saveIconConrtoller;

    [SerializeField] private string id;

    private bool shieldCollected = false;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private ParticleSystem particles;
    

    public AudioClip hurtClip;
    public AudioClip UIClip;

    private void Awake()
    {
        
        controls = PlayerData.getControls();
        interactHintTrigger = GetComponent<InteractHintTrigger>();
        saveIconConrtoller = GameObject.FindGameObjectWithTag("SaveIconController").GetComponent<DisplaySaveIcon>();
        arenaBattle = FindFirstObjectByType<ArenaBattleTrigger>();

        if (id == null)
        {
            Debug.Log("Id of Shield Pickup is null!");
        }
        else
        {
            var room = SaveSystem.getRoom(gameObject.scene.name);

            if (room.pickups.TryGetValue(id, out bool collected) && collected)
            {
                Debug.Log("shield collected previously");
                interactHintTrigger.SetInteractPopupActive(false);
                interactHintTrigger.shouldCheckForCollision = false;
                shieldCollected = true;
                Destroy(gameObject);
            }
        }
    }

    protected override IEnumerator QuicktimeEventCoroutine()
    {

        interactHintTrigger.SetInteractPopupActive(true);
        interactHintTrigger.interactText = "";
        
        FaderController.instance.fadeDuration = 1f;
        yield return FaderController.instance.FadeToWhite();
        yield return new WaitForSecondsRealtime(0.8f);
        
        interactHintTrigger.shouldCheckForCollision = false;
        interactHintTrigger.SetInteractPopupActive(false);

        yield return AbilityObtainedUI.instance.FadeInAbilityScreen(1, 1, "Obtained a Shield", "", "Right Click to Block Incoming Attacks");

        FaderController.instance.fadeDuration = 0.5f;

        sprite.enabled = false;
        particles.gameObject.SetActive(false);


        yield return FaderController.instance.FadeFromWhite();


        yield return new WaitForSecondsRealtime(0.7f);

        AudioSource.PlayClipAtPoint(UIClip, transform.position, 5.0f);
        StartCoroutine(saveIconConrtoller.DisplaySaveIconCoroutine());

        

        if (id == null)
        {
            Debug.Log("Id of shield is null!");
        }
        else
        {
            var room = SaveSystem.getRoom(gameObject.scene.name);
            room.pickups[id] = true;
        }

        PlayerData.shieldUnlocked = true;
        SaveSystem.Save(PlayerData.saveIndex);



        yield return new WaitForSeconds(0.5f);

        EndQuickTimeEvent();

        yield return new WaitForSeconds(3.0f);

        arenaBattle.StartArenaBattle();

        PlayerHealthManager.instance.PlayBlockTutorial();

        Destroy(gameObject.transform.parent.gameObject);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !shieldCollected)
        {
            if (!used)
            {
                interactHintTrigger.SetInteractPopupActive(true);
                PlayerContextActionInputManager.instance.SetInteractable(this);
            }
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

    public void Interact()
    {
        interactHintTrigger.SetInteractPopupActive(false);
        StartQuicktimeEvent();

        used = true;
    }
}
