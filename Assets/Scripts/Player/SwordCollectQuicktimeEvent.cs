using System.Collections;
using UnityEngine;

public class SwordCollectEvent : QuicktimeEvent
{

    private InteractHintTrigger interactHintTrigger;
    private bool interactPressed;
    private bool used = false;
    private CamShakeSource camShakeSource;

    private void Awake()
    {
        controls = PlayerData.getControls();
        interactHintTrigger = GetComponent<InteractHintTrigger>();
        controls.Player.Interact.performed += ctx => interactPressed = true;
        camShakeSource = GameObject.FindGameObjectWithTag("CinemachineImpulseSource").GetComponent<CamShakeSource>();
    }

    protected override IEnumerator QuicktimeEventCoroutine()
    {

        interactPressed = false;
        for (int i = 0; i < 3; i++)
        {
            interactHintTrigger.interactText = "";
            yield return new WaitForSecondsRealtime(0.9f);
            interactHintTrigger.SetInteractPopupActive(true);

            
            while(interactPressed == false)
            {
                yield return null;
            }
            interactHintTrigger.SetInteractPopupActive(false);
            camShakeSource.AddVerticalScreenShake(1.0f);
            interactPressed = false;
        }
        interactHintTrigger.shouldCheckForCollision = false;
        interactHintTrigger.SetInteractPopupActive(false);

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
