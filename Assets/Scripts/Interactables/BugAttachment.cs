using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstraintManager;

public class BugAttachment : MonoBehaviour, IInteractable
{
    [SerializeField]
    private GameObject bugIndicator;
    private Animator bugIndicatorAnimator;

    private bool bugIsAttached = false;

    private Timer listenTimer;

    private AbstractActivity currentActivity;

    private AudioSource targetAudioSource = null;

    private float timeListened = 0;
    
    [SerializeField]
    private AudioClip rewardClip;

    private bool grantedReward = false;
    private GameConstraint constraintRewarded = GameConstraint.none;
    void Awake()
    {
        listenTimer = gameObject.AddComponent<Timer>();
        bugIndicator.SetActive(bugIsAttached);
        bugIndicatorAnimator = bugIndicator.GetComponent<Animator>();
    }

    void Start()
    {
    }

    public void SetCurrentActivity(AbstractActivity activity)
    {

        timeListened = 0;
        string lastActivityString = activity.GetActivityString();
        currentActivity = activity;


        if (targetAudioSource != null)
        {
            ConstraintManager.GameConstraint? constraint = activity.GetGameConstraint();

            if (constraint.HasValue)
            {
                listenTimer.SetPaused(false);

                listenTimer.Init(currentActivity.GetNeededTimeToListen(), ReachedConstraintTime);
            }
                StartPlayingSoundAt(targetAudioSource, activity.GetAudioClip(), 0f, activity.IsContinuous());

            if (currentActivity.GetActivityString().Length > 0)
            {
                DisplayActivityString();
            }
        }
    }

    private void ReachedConstraintTime()
    {
        constraintRewarded = currentActivity.GetGameConstraint().Value;
        ConstraintManager.Instance.SetSatisfied(currentActivity.GetGameConstraint().Value);
        grantedReward = true;
    }

    public bool HasBugAttached()
    {
        return bugIsAttached;
    }

    public void RemoveBug()
    {
        bugIsAttached = false;
        bugIndicator.SetActive(false);
        bugIndicatorAnimator.SetBool("isActive", false);
    }

    public void AddBug(Vector3 fromPosition)
    {
        bugIsAttached = true;
        bugIndicator.SetActive(true);
        bugIndicatorAnimator.SetBool("isActive", false);
        float xPos = fromPosition.x < transform.position.x ? -0.65f : 0.65f;

        //debugBug.transform.position = transform.root.position + new Vector3(xPos, 0, 0);
        ConstraintManager.Instance.SetSatisfied(ConstraintManager.GameConstraint.bugUsed);
    }

    public void Interact(PlayerInteraction interactingPlayer)
    {
        //go the extra tour around the player 
        if (bugIsAttached)
        {
            interactingPlayer.RemoveBugFrom(this);
        }
        else
        {
            interactingPlayer.PutBugOn(this);
        }
    }

    public string GetInteractionTypeString()
    {
        if (bugIsAttached)
        {
            return "detach the bug";
        }
        else
        {
            return "attach a bug";
        }
    }

    public void StartListening(AudioSource source)
    {
        ConstraintManager.GameConstraint? constraint = currentActivity.GetGameConstraint();

        if (constraint.HasValue && currentActivity.GetNeededTimeToListen() < timeListened)
        {
            listenTimer.Init(currentActivity.GetNeededTimeToListen() - timeListened, ReachedConstraintTime);
            listenTimer.SetPaused(false);
        }
        this.targetAudioSource = source;
        //this.audioSource.spatialBlend = 0.2f;
        StartPlayingSoundAt(source, 
            currentActivity.GetAudioClip(), 
            currentActivity.GetTimeProgress(), 
            currentActivity.IsContinuous());

        if (currentActivity.GetActivityString().Length > 0)
        {
            DisplayActivityString();
        }
        bugIndicatorAnimator.SetBool("isActive", true);

    }

    public void StopListening()
    {
        ConstraintManager.GameConstraint? constraint = currentActivity.GetGameConstraint();

        if (constraint.HasValue && currentActivity.GetNeededTimeToListen() < timeListened)
        {
            timeListened = listenTimer.GetTimePassed();
            listenTimer.SetPaused(true);
        }
        HUDManager.Instance.StopListenContent();
        //this.audioSource.spatialBlend = 1;
        this.targetAudioSource.Stop();

        if (grantedReward)
        {
            StartPlayingSoundAt(targetAudioSource, rewardClip, 0, false);
            HUDManager.Instance.DisplayMessage(ConstraintManager.ConstraintToRewardString(constraintRewarded));
            grantedReward = false;
        }
        targetAudioSource = null;
        bugIndicatorAnimator.SetBool("isActive", false);
    }

    public void StartPlayingSoundAt(AudioSource source, AudioClip clip, float clipTimePosition, bool looping)
    {
        if(source.clip == clip && looping)
        {
            return;
        }
        source.clip = clip;
        source.loop = looping;
        source.time = Mathf.Min(clipTimePosition > 0 ? clipTimePosition : 0f, source.clip.length-1);
        source.Play();
    }

    public void DisplayActivityString()
    {
        string activityString = currentActivity.GetActivityString();
        if (activityString.StartsWith(DialogueManager.DIALOGUE_INDICATOR))
        {
            DialogueHolder.DialogueKey? dialogueKey = DialogueManager.ToDialogueKey(activityString.Substring(2).Trim());
            if (dialogueKey.HasValue)
            {
                Dialogue dialogue = DialogueManager.Instance.GetDialogueTemplate(dialogueKey.Value).ToDialogue();
                    if (dialogue != null)
                    {
                        HUDManager.Instance.ShowSkippedContent(dialogue.GetFullText(),
                               currentActivity.GetTimeProgress(),
                               currentActivity.GetFullDisplayTime(),
                               currentActivity.IsContinuous());
                        return;
                    }
            }
        }

        HUDManager.Instance.ShowSkippedContent(currentActivity.GetActivityString(),
           currentActivity.GetTimeProgress(),
           currentActivity.GetFullDisplayTime(),
           currentActivity.IsContinuous());
    }
}