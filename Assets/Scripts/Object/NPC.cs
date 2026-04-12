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

    public DialogueScriptableObj dialogueEntries;

    private bool currentlySpeaking = false;

    public string npcId;



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
            if (!currentlySpeaking)
            {
                interactHintTrigger.SetInteractPopupActive(true);

                if (interactPressed)
                {
                    interactHintTrigger.SetInteractPopupActive(false);
                    interactHintTrigger.shouldCheckForCollision = false;
                    currentlySpeaking = true;
                    interactPressed = false;
                    DisplayDialogue();
                }
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactHintTrigger.SetInteractPopupActive(false);
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

        yield return DialogueUI.instance.DisplayDialogueChain(GetBestDialogue().text);
        //interactHintTrigger.SetInteractPopupActive(true);
        currentlySpeaking = false;
        interactHintTrigger.shouldCheckForCollision = true;
        interactPressed = false;
        PlayerData.MarkTalkedTo(npcId);
    }


    public DialogueEntry GetBestDialogue()
    {
        DialogueEntry best = null;



        foreach (var entry in dialogueEntries.entries)
        {
            /*if (entry.condition != null)
                Debug.Log(entry.condition.IsMet(npcId));*/
            if (entry.condition == null || entry.condition.IsMet(npcId))
            {
                if (best == null || entry.priority > best.priority)
                {
                    best = entry;
                }
            }
        }

        return best;
    }
}
