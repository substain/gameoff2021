using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugAttachment : MonoBehaviour, IInteractable
{
    [SerializeField]
    private List<AudioClip> activitySoundClips;

    [SerializeField]
    private GameObject debugBug;

    private bool bugIsAttached = false;

    private Timer activityTimer;

    private int activityIndex = 0;

    void Awake()
    {
        activityTimer = gameObject.AddComponent<Timer>();
        debugBug.SetActive(bugIsAttached);
    }

    public void StartActivity(int activityIndex, float activityDuration)
    {
        this.activityIndex = activityIndex;
        activityTimer.Init(activityDuration);
    }

    public AudioClip GetCurrentAudioClip()
    {
        return activitySoundClips[activityIndex];
    }

    public float GetCurrentAudioClipPos()
    {
        return activityTimer.GetTimePassed();
    }

    public bool HasBugAttached()
    {
        return bugIsAttached;
    }

    public void RemoveBug()
    {
        bugIsAttached = false;
        debugBug.SetActive(false);
    }

    public void AddBug(Vector3 fromPosition)
    {
        bugIsAttached = true;
        debugBug.SetActive(true);
        float xPos = fromPosition.x < transform.position.x ? -0.65f : 0.65f;

        debugBug.transform.position = transform.root.position + new Vector3(xPos, 0, 0);
    }

}