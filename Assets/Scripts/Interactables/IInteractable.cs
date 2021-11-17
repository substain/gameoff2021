using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// Interact with this interactable
    /// </summary>
    public void Interact(PlayerInteraction interactingPlayer);

    /// <summary>
    /// Returns a string describing what interacting with this interactable does.
    /// </summary>
    public string GetInteractionTypeString();
}