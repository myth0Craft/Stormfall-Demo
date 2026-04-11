using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private InteractHintTrigger interactHintTrigger;
    private bool interactPressed = false;
    private PlayerControls controls;

    public bool shouldMovePlayerToPosition = false;
    public Vector2 playerPositionOffset = Vector2.zero;

    public List<string> dialogue = new List<string>();

    private bool currentlySpeaking = false;



    private void Awake()
    {
        controls = PlayerData.getControls();
        interactHintTrigger = GetComponent<InteractHintTrigger>();
        controls.Player.Interact.performed += ctx => interactPressed = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (interactPressed && !currentlySpeaking)
            {
                interactHintTrigger.SetInteractPopupActive(false);
                interactHintTrigger.shouldCheckForCollision = false;
                currentlySpeaking = true;
                interactPressed = false;
                DisplayDialogue();
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

    private void DisplayDialogue()
    {
        StartCoroutine(DisplayDialogueCoroutine());
    }

    private IEnumerator DisplayDialogueCoroutine()
    {
        if (shouldMovePlayerToPosition)
        {
            yield return PlayerMovement.instance.MoveHorizontalToPosition(transform.position.x + playerPositionOffset.x);
            if (!PlayerMovement.instance.getFacingDirection())
            {
                PlayerMovement.instance.TurnSprite();
                yield return PlayerMovement.instance.MoveHorizontalToPosition(transform.position.x + playerPositionOffset.x);
            }
        }

        yield return DialogueUI.instance.DisplayDialogueChain(dialogue);
        currentlySpeaking = false;
        interactHintTrigger.shouldCheckForCollision = true;
        interactPressed = false;
        interactHintTrigger.SetInteractPopupActive(true);
    }
}
