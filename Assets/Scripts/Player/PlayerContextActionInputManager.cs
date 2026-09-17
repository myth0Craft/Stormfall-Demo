using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;


public enum Context
{
    Parry,
    Interact,
    UI
}
public class PlayerContextActionInputManager : MonoBehaviour
{
    public static PlayerContextActionInputManager instance;

    private PlayerControls controls;

    private Context previousContext;


    private IInteractable currentInteractable;

    public Context currentContext { get; private set; } = Context.Parry;

    public void SetUIContext()
    {
        previousContext = currentContext;
        this.currentContext = Context.UI;
    }

    public void RestoreContext()
    {
        currentContext = previousContext;
    }

    public void SetParryContext()
    {
        this.currentContext = Context.Parry;
    }

    public void SetInteractContext()
    {
        this.currentContext = Context.Interact;
    }



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
            return;
        }

        controls = PlayerData.getControls();
    }

    private void OnEnable()
    {
        controls.Player.ContextAction.performed += OnContextActionPerformed;
    }

    private void OnDisable()
    {
        controls.Player.ContextAction.performed -= OnContextActionPerformed;
    }

    private void OnContextActionPerformed(InputAction.CallbackContext context)
    {
        switch (this.currentContext)
        {
            case Context.Parry:
                PlayerMovement.instance.OnBlockPressed();
                return;
            case Context.Interact:
                currentInteractable.Interact();
                return;
            case Context.UI:
                return;
        }
    }

    public void SetInteractable(IInteractable interactable)
    {
        currentInteractable = interactable;
        currentContext = Context.Interact;
    }

    public void ClearInteractable(IInteractable interactable)
    {
        currentInteractable = null;
        currentContext = Context.Parry;
    }
}
