using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public abstract class MultiMenuItem : MenuItem
{
    private bool isFocused = false;

    private List<SingleMenuItem> childrenItems;
    protected int selectedItemsIndex = 0;

    protected virtual void Awake()
    {
        childrenItems = GetComponentsInChildren<SingleMenuItem>().OrderBy(bs => bs.transform.position.x).ToList();
    }

    public override void SelectNext()
    {
        SetIndexSelected(selectedItemsIndex + 1);
    }

    public override void SelectPrevious()
    {
        SetIndexSelected(selectedItemsIndex - 1);
    }

    public void SetIndexSelected(int index)
    {
        selectedItemsIndex = Mathf.Clamp(index, 0, childrenItems.Count - 1);
        UpdateItemFocus();
    }

    private void UpdateItemFocus()
    {
        for (int i = 0; i < childrenItems.Count; i++)
        {
            if (i == selectedItemsIndex)
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
        this.isFocused = isFocused;
        UpdateItemFocus();
    }
}