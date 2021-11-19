using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This is implemented as special case of a dialogue
/// </summary>
public class MonologueHolder : DialogueHolder
{
    private const int SECONDS_TO_WAIT = 1;

    private bool isUsableMonologue = false;

    private PlayerInteraction interactingPlayer;

    void Awake()
    {
        interactingPlayer = GetComponentInParent<PlayerInteraction>();
    }

    protected override void CheckAvailableDialogues()
    {
        base.CheckAvailableDialogues();
        if(availableOneshots.Count > 0)
        {
            StartCoroutine(StartMonologueDelayed());
        }

        if(availableRepeatables.Count > 0)
        {
            string repeatables = "";
            availableRepeatables.ForEach(ar => repeatables += ar.GetKey().ToString() + " ");
            Debug.LogWarning("MonologueHolder does not support repeatables to avoid being stuck in dialogues. Unsupported keys: " + repeatables);
        }
    }

    IEnumerator StartMonologueDelayed()
    {
        yield return new WaitForSeconds(SECONDS_TO_WAIT);
        StartMonologue();
    }

    private void StartMonologue()
    {
        Debug.Log("starting monologue...");
        isUsableMonologue = true;
        interactingPlayer.SetCurrentInteractable(this);
        Interact(interactingPlayer);
    }
    
    public override string GetInteractionTypeString()
    {
        return "continue";
    }

    public override void CancelDialogue()
    {   
        //monologues cannot be canceled.
        return;
    }

    protected override void FinishDialogue(PlayerInteraction interactingPlayer)
    {
        isUsableMonologue = false;
        base.FinishDialogue(interactingPlayer);
    }

    public override bool HasValidNewDialogue()
    {
        return isUsableMonologue && base.HasValidNewDialogue();
    }

    public override bool IsInRange(Transform playerTransform)
    {
        return true;
    }
}