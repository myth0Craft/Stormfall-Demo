using System.Collections;
using UnityEngine;

public class SwordCollectEvent : QuicktimeEvent
{

    private InteractHintTrigger interactHintTrigger;
    private bool interactPressed;
    private bool used = false;
    private CamShakeSource camShakeSource;

    private DisplaySaveIcon saveIconConrtoller;

    private void Awake()
    {
        controls = PlayerData.getControls();
        interactHintTrigger = GetComponent<InteractHintTrigger>();
        controls.Player.Interact.performed += ctx => interactPressed = true;
        camShakeSource = GameObject.FindGameObjectWithTag("CinemachineImpulseSource").GetComponent<CamShakeSource>();
        saveIconConrtoller = GameObject.FindGameObjectWithTag("SaveIconController").GetComponent<DisplaySaveIcon>();
    }

    protected override IEnumerator QuicktimeEventCoroutine()
    {

        interactPressed = false;
        for (int i = 0; i < 3; i++)
        {
            interactHintTrigger.interactText = "";
            yield return new WaitForSecondsRealtime(0.9f);
            interactHintTrigger.SetInteractPopupActive(true);

            yield return new WaitForSecondsRealtime(0.3f);
            while(interactPressed == false)
            {
                yield return null;
            }
            interactHintTrigger.SetInteractPopupActive(false);
            camShakeSource.AddVerticalScreenShake(0.8f);
            interactPressed = false;
        }
        interactHintTrigger.shouldCheckForCollision = false;
        interactHintTrigger.SetInteractPopupActive(false);

        yield return new WaitForSecondsRealtime(0.3f);
        PlayerData.currentScene = gameObject.scene.name;
        PlayerData.posX = gameObject.transform.position.x;
        PlayerData.posY = gameObject.transform.position.y;
        PlayerData.swordUnlocked = true;
        SaveSystem.Save(PlayerData.saveIndex);
        StartCoroutine(saveIconConrtoller.DisplaySaveIconCoroutine());

        EndQuickTimeEvent();
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
            if (!used)
            {
                interactHintTrigger.SetInteractPopupActive(true);
                if (interactPressed)
                {
                    interactHintTrigger.SetInteractPopupActive(false);
                    camShakeSource.AddVerticalScreenShake(0.7f);
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
