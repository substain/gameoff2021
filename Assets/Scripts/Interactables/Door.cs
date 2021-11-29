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

    [SerializeField]
    private List<AudioClip> doorOpenClips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> doorCloseClips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> doorTryLockedClips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> doorUnlockClips = new List<AudioClip>();

    private bool isClosed = true;

    private Timer timer;
    private AudioSource source;

    void Awake()
    {
        timer = gameObject.AddComponent<Timer>();
        source = gameObject.AddComponent<AudioSource>();
    }

    public void Interact(PlayerInteraction interactingPlayer)
    {
        if (isClosed && IsKeyProtected() && !interactingPlayer.HasKeyWithId(neededKeyId))
        {
            HUDManager.Instance.DisplayMessage("You don't have the right key.");
            Util.PlayRandomFromList(doorTryLockedClips, source, false);
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
        if (IsKeyProtected())
        {
            Util.PlayRandomFromList(doorUnlockClips, source, false);
        }
        else
        {
            Util.PlayRandomFromList(doorOpenClips, source, false);
        }

        isClosed = false;
        doorObject.SetActive(false);

        if(stayOpenDuration > 0)
        {
            timer.Init(stayOpenDuration, CloseDoor);
        }
    }

    public void CloseDoor()
    {
        Util.PlayRandomFromList(doorCloseClips, source, false);

        isClosed = true;
        doorObject.SetActive(true);
    }

    public string GetInteractionTypeString()
    {
        if (isClosed)
        {
            return "open the door";
        }
        else
        {
            return "close the door";
        }
    }

    public bool IsKeyProtected()
    {
        return neededKeyId != -1;
    }
}
