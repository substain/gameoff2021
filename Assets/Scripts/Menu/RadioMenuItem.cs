using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public abstract class RadioMenuItem : MenuItem
{
    private readonly Color focusedTextColor = HUDDialogueDisplay.GetPersonColor(Person.Sting);
    private readonly Color unfocusedTextColor = Color.white;
    private bool isFocused = false;
    private Text title;

    protected List<SelectableMenuItem> childrenItems;
    protected int focusedItemsIndex = 0;

    protected virtual void Awake()
    {
        childrenItems = GetComponentsInChildren<SelectableMenuItem>().OrderBy(bs => bs.transform.position.x).ToList();
        title = GetComponentsInChildren<Text>()[0];
    }

    protected virtual void Start()
    {
        InitSelection();
    }

    public override UIManager.MenuSoundType GetSoundType()
    {
        return childrenItems[focusedItemsIndex].GetSoundType();
    }

    public override void FocusNext()
    {
        SetIndexFocused(focusedItemsIndex + 1);
    }

    public override void FocusPrevious()
    {
        SetIndexFocused(focusedItemsIndex - 1);
    }

    public void SetIndexFocused(int index)
    {
        focusedItemsIndex = Mathf.Clamp(index, 0, childrenItems.Count - 1);
        UpdateItemFocus();
    }

    private void UpdateItemFocus()
    {
        for (int i = 0; i < childrenItems.Count; i++)
        {
            if (i == focusedItemsIndex)
            {
                childrenItems[i].SetFocused(isFocused);
            }
            else
            {
                childrenItems[i].SetFocused(false);
            }
        }
    }

    public override void SetFocused(bool isFocused)
    {
        title.color = isFocused ? focusedTextColor : unfocusedTextColor;
        this.isFocused = isFocused;
        UpdateItemFocus();
    }

    public override void UseFocused()
    {
        if (childrenItems[focusedItemsIndex].IsSelected())
        {
            return;
        }
        childrenItems[focusedItemsIndex].UseFocused();
        for (int i = 0; i < childrenItems.Count; i++)
        {
            if (i != focusedItemsIndex)
            {
                childrenItems[i].SetSelected(false);
            }
        }
    }

    protected virtual void InitSelection()
    {
        focusedItemsIndex = 0;
    }

}