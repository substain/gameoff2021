using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeableItem : MonoBehaviour, IInteractable
{
    public enum ItemType
    {
        key, bug
    }

    [Tooltip("Type of the item that is taken by the player")]
    [SerializeField]
    private ItemType itemType;

    [Tooltip("Id of the item to give the player")]
    [SerializeField]
    private int id = -1;

    public void Interact(PlayerInteraction interactingPlayer)
    {
        interactingPlayer.AddItem(itemType, id);
        Destroy(this.gameObject);
    }

    public string GetInteractionTypeString()
    {
        return "pick up the " + itemType.ToString();
    }
}
