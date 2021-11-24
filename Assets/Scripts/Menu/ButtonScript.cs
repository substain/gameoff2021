using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public bool selected;
    [SerializeField] ButtonScript[] otherButtonsInGroup;
    [SerializeField] Animator anim = null;
    [SerializeField] protected bool isStartButton = false;

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
            Select();
        }
        else
        {
            Deselect();
        }
    }

    public virtual void Select() {
        selected = true;
        if (anim != null)
        {
            anim.Play("Selected");
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
        //GlobalSound.Play("MenuButtonActivated.mp3");
    }
}
