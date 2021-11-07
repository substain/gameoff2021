using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Tooltip("The door object to control")]
    [SerializeField]
    private GameObject doorObject;

    [Tooltip("The duration for which the door stays open. if stayOpenDuration < 0, the door does not close")]
    [SerializeField]
    private float stayOpenDuration = 2.0f;

    [Tooltip("The id of the key that is needed for the door to open. -1 means no key is needed")]
    [SerializeField]
    private int neededKeyId = -1;


    private bool isClosed = true;

    private Timer timer;


    void Awake()
    {
        timer = gameObject.AddComponent<Timer>();
    }

    public void Interact(PlayerInteraction interactingPlayer)
    {
        if (isClosed && neededKeyId != -1 && !interactingPlayer.HasKeyWithId(neededKeyId))
        {
            //door is closed and needs a key, which the player doesnt have -> don't open the door
            return;
        }
        if (isClosed)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }

    }

    public void OpenDoor()
    {
        isClosed = false;
        doorObject.SetActive(false);

        if(stayOpenDuration > 0)
        {
            timer.Init(stayOpenDuration, CloseDoor);
        }
    }

    public void CloseDoor()
    {
        isClosed = true;
        doorObject.SetActive(true);
    }
}
