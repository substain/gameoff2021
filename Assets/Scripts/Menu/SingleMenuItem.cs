using UnityEngine;
using UnityEngine.UI;

public abstract class SingleMenuItem : MenuItem
{
    private readonly Color focusedTextColor = HUDDialogueDisplay.GetPersonColor(Person.Sting);
    private readonly Color unfocusedTextColor = Color.white;
    private Text buttonText;

    protected virtual void Awake()
    {
        this.buttonText = GetComponentInChildren<Text>();
    }

    public override void SelectNext()
    {
        //nothing to do here - this item is a leaf
        return;
    }

    public override void SelectPrevious()
    {
        //nothing to do here - this item is a leaf
        return;
    }

    public override void SetFocused(bool isFocused)
    {
        buttonText.color = isFocused ? focusedTextColor : unfocusedTextColor;
    }
}