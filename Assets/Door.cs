using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField]
    private GameObject doorObject;

    [Tooltip("The id of the key that is needed for the door to open")]
    [SerializeField]
    private int? neededKeyId = null;

    private bool isClosed = true;

    public void Interact(PlayerInteraction interactingPlayer)
    {
        if (isClosed && neededKeyId.HasValue && !interactingPlayer.HasKeyWithId(neededKeyId.Value))
        {
            //door is closed and needs a key, which the player doesnt have -> don't open the door
            return;
        }
        isClosed = !isClosed;
        doorObject.SetActive(isClosed);
    }
}
