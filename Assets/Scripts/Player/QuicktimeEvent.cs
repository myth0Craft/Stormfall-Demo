using System.Collections;
using UnityEngine;

public class QuicktimeEvent : MonoBehaviour
{
    protected PlayerControls controls;
    protected bool eventActive = false;

    private void Awake()
    {
        controls = PlayerData.getControls();        
    }

    protected void StartQuicktimeEvent()
    {
        disableControls();
        EnableSpecificInput();
        eventActive = true;
        StartCoroutine(QuicktimeEventCoroutine());
    }

    protected virtual IEnumerator QuicktimeEventCoroutine()
    {
        yield return null;
    }

    //private void Update()
    //{
    //    if (eventActive)
    //    {

    //    }
    //}

    protected void EndQuickTimeEvent()
    {
        controls.Player.Enable();
    }

    protected virtual void EnableSpecificInput()
    {
        //call controls.Player.controlX.Enable();
    }

    private void disableControls()
    {
        controls.Player.Disable();
    }
}
