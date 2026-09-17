using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    private InteractHintTrigger interactHintTrigger;

    public bool shouldMovePlayerToPosition = false;
    public Vector2 playerPositionOffset = Vector2.zero;

    public DialogueScriptableObj dialogueEntries;

    private bool currentlySpeaking = false;

    public string npcId;

    public void Interact()
    {
        if (currentlySpeaking)
            return;

        interactHintTrigger.SetInteractPopupActive(false);
        interactHintTrigger.shouldCheckForCollision = false;
        currentlySpeaking = true;

        DisplayDialogue();
    }

    private void Awake()
    {
        interactHintTrigger = GetComponent<InteractHintTrigger>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerContextActionInputManager.instance.SetInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerContextActionInputManager.instance.ClearInteractable(this);
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
        PlayerData.MarkTalkedTo(npcId);
        PlayerContextActionInputManager.instance.SetInteractable(this);
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
