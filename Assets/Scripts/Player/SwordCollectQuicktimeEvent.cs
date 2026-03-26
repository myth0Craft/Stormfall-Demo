using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SwordCollectEvent : QuicktimeEvent
{

    private InteractHintTrigger interactHintTrigger;
    private bool interactPressed;
    private bool used = false;
    private CamShakeSource camShakeSource;

    private DisplaySaveIcon saveIconConrtoller;

    public ParticleSystem swirlParticles;

    public GameObject explosionParticles;

    private GameObject particleInstance;

    private Light2D bgLight;

    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite spriteToSwitch;

    private float intensity;

    [SerializeField] private string id;

    private bool swordCollected = false;

    private TextMeshProUGUI text;

    public AudioClip hurtClip;
    public AudioClip UIClip;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.gameObject.SetActive(false);
        controls = PlayerData.getControls();
        interactHintTrigger = GetComponent<InteractHintTrigger>();
        controls.Player.Interact.performed += ctx => interactPressed = true;
        camShakeSource = GameObject.FindGameObjectWithTag("CinemachineImpulseSource").GetComponent<CamShakeSource>();
        saveIconConrtoller = GameObject.FindGameObjectWithTag("SaveIconController").GetComponent<DisplaySaveIcon>();
        //swirlParticles = GetComponentInChildren<ParticleSystem>();
        swirlParticles.gameObject.SetActive(false);
        bgLight = GetComponentInChildren<Light2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        intensity = bgLight.intensity;

        if (id == null)
        {
            Debug.Log("Id of Sword Pickup is null!");
        }
        else
        {
            var room = SaveSystem.getRoom(gameObject.scene.name);

            if (room.pickups.TryGetValue(id, out bool collected) && collected)
            {
                Debug.Log("sword collected previously");
                interactHintTrigger.SetInteractPopupActive(false);
                interactHintTrigger.shouldCheckForCollision = false;
                swordCollected = true;
                spriteRenderer.sprite = spriteToSwitch;
            }
        }
    }

    protected override IEnumerator QuicktimeEventCoroutine()
    {

        yield return PlayerMovement.instance.MoveHorizontalToPosition(transform.position.x + 0.45f);
        if (PlayerMovement.instance.getFacingDirection())
        {
            PlayerMovement.instance.TurnSprite();
            yield return PlayerMovement.instance.MoveHorizontalToPosition(transform.position.x + 0.45f);
        }

        PlayerAnimationManager.instance.SetGainSwordAbility(true);
        bgLight.intensity = 5;
        interactPressed = false;
        swirlParticles.gameObject.SetActive(true);
        
        for (int i = 0; i < 5; i++)
        {
            interactHintTrigger.interactText = "";
            yield return new WaitForSecondsRealtime(0.9f);
            Destroy(particleInstance);
            interactHintTrigger.SetInteractPopupActive(true);
            AudioSource.PlayClipAtPoint(hurtClip, transform.position, 5.0f);
            yield return new WaitForSecondsRealtime(0.3f);
            while(interactPressed == false)
            {
                yield return null;
            }

            AudioSource.PlayClipAtPoint(UIClip, transform.position);
            
            
            CamShakeSource.instance.AddScreenShake(0.2f);
            ContinuousCameraShakeSource.instance.currentForce *= 1.5f;
            bgLight.intensity *= 1.5f;
            bgLight.pointLightOuterRadius *= 1.2f;
            bgLight.pointLightInnerRadius *= 1.2f;
            interactHintTrigger.SetInteractPopupActive(false);
            /*camShakeSource.AddVerticalScreenShake(0.8f);
            yield return new WaitForSecondsRealtime(0.1f);
            camShakeSource.AddVerticalScreenShake(0.8f);
            yield return new WaitForSecondsRealtime(0.1f);
            camShakeSource.AddVerticalScreenShake(0.8f);*/
            if (particleInstance != null)
            {
                Destroy(particleInstance);
            }
            particleInstance = Instantiate(
                explosionParticles,
                transform.position,
                Quaternion.identity
                );

            interactPressed = false;
        }
        FaderController.instance.fadeDuration = 1f;
        yield return FaderController.instance.FadeToWhite();
        ContinuousCameraShakeSource.instance.StopAllCoroutines();
        CamShakeSource.instance.StopAllCoroutines();
        yield return new WaitForSecondsRealtime(0.8f);
        
        bgLight.intensity = 0;
        bgLight.gameObject.SetActive(false);
        Destroy(particleInstance);
        interactHintTrigger.shouldCheckForCollision = false;
        interactHintTrigger.SetInteractPopupActive(false);
        swirlParticles.gameObject.SetActive(false);

        yield return AbilityObtainedUI.instance.FadeInAbilityScreen(0, 0, "Obtained a Sword", "", "Left Click to Attack");

        FaderController.instance.fadeDuration = 0.5f;
        yield return FaderController.instance.FadeFromWhite();


        yield return new WaitForSecondsRealtime(0.7f);

        PlayerAnimationManager.instance.SetGainSwordAbility(false);
        spriteRenderer.sprite = spriteToSwitch;
        AudioSource.PlayClipAtPoint(UIClip, transform.position, 5.0f);
        StartCoroutine(saveIconConrtoller.DisplaySaveIconCoroutine());

        

        if (id == null)
        {
            Debug.Log("Id of Waystone is null!");
        }
        else
        {
            var room = SaveSystem.getRoom(gameObject.scene.name);
            room.pickups[id] = true;
        }

        PlayerData.swordUnlocked = true;
        SaveSystem.Save(PlayerData.saveIndex);

        EndQuickTimeEvent();


        yield return new WaitForSeconds(2.0f);

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



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !swordCollected)
        {
            interactPressed = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !swordCollected)
        {
            if (!used)
            {
                interactHintTrigger.SetInteractPopupActive(true);
                if (interactPressed)
                {
                    interactHintTrigger.SetInteractPopupActive(false);
                    ContinuousCameraShakeSource.instance.AddScreenShakeOverTime(0.3f, 200000f, 0.1f);
                    interactPressed = false;
                    StartQuicktimeEvent();
                    
                    used = true;
                }
            }
            
        }
    }

    protected override void EnableSpecificInput()
    {
        this.controls.Player.Interact.Enable();
    }
}
