using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public enum MenuSound
    {
        select, useSelected
    }

    public bool selected;
    [SerializeField] ButtonScript[] otherButtonsInGroup;
    [SerializeField] Animator anim = null;
    [SerializeField] protected bool isStartButton = false;
    private AudioSource audioSource;
    private AudioClip selectAudioClip;
    private AudioClip useSelectedAudioClip;

    protected virtual void Start() {

        if (anim == null) {
            GetComponent<Animator>();
        }
        InitSelection();
    }

    public virtual void InitSelection()
    {
        if (isStartButton)
        {
            Select(playSound: false);
        }
        else
        {
            Deselect();
        }
    }

    public virtual void Select(bool playSound = true) {
        selected = true;
        if (anim != null)
        {
            anim.Play("Selected");
        }

        if (playSound)
        {
            audioSource?.PlayOneShot(selectAudioClip);
        }
        //GlobalSound.Play("MenuButtonSelectionChanged.mp3");

        /*foreach (ButtonScript otherButton in otherButtonsInGroup) {
            otherButton.Deselect();
        }*/
    }

    public virtual void Deselect() {
        selected = false;
        if (anim != null)
        {
            anim.Play("Deselected");
        }
    }

    public void Activate() {
        if (anim != null)
        {
            anim.Play("Activated");
        }
        audioSource?.PlayOneShot(useSelectedAudioClip);

        //GlobalSound.Play("MenuButtonActivated.mp3");
    }

    public void SetAudioSource(AudioSource audioSource)
    {
        this.audioSource = audioSource;
    }

    public void SetAudioClips(AudioClip selectClip, AudioClip useSelectedClip)
    {
        this.selectAudioClip = selectClip;
        this.useSelectedAudioClip = useSelectedClip;
    }
}
