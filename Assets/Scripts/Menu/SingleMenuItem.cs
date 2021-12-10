using UnityEngine;
using UnityEngine.UI;

public abstract class SingleMenuItem : MenuItem
{
    private readonly Color focusedTextColor = HUDDialogueDisplay.GetPersonColor(Person.Sting);
    private readonly Color unfocusedTextColor = Color.white;
    protected Text menuItemText;

    protected virtual void Awake()
    {
        this.menuItemText = GetComponentInChildren<Text>();
    }

    public override void FocusNext()
    {
        //nothing to do here - this item is a leaf
        return;
    }

    public override void FocusPrevious()
    {
        //nothing to do here - this item is a leaf
        return;
    }

    public override void SetFocused(bool isFocused)
    {
        menuItemText.color = isFocused ? focusedTextColor : unfocusedTextColor;
    }
}