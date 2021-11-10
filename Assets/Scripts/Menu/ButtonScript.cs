using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public bool selected;
    [SerializeField] ButtonScript[] otherButtonsInGroup;
    [SerializeField] Animator anim;
    [SerializeField] bool isStartButton = false;

    void Start() {
        if (anim == null) {
            GetComponent<Animator>();
        }

        if (isStartButton) {
            Select();
        } else {
            Deselect();
        }
    }

    public void Select() {
        selected = true;
        anim.Play("Selected");
        //GlobalSound.Play("MenuButtonSelectionChanged.mp3");

        foreach (ButtonScript otherButton in otherButtonsInGroup) {
            otherButton.Deselect();
        }
    }

    public void Deselect() {
        selected = false;
        anim.Play("Deselected");
    }

    public void Activate() {
        anim.Play("Activated");
        //GlobalSound.Play("MenuButtonActivated.mp3");
    }
}
