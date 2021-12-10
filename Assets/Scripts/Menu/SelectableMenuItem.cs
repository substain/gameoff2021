using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class SelectableMenuItem : SingleMenuItem
{
    private const float FONT_SIZE_FACTOR = 1.35f;
    private int initialFontSize;
    private bool selected;

    protected override void Awake()
    {
        base.Awake();
        initialFontSize = menuItemText.fontSize;
    }

    public override UIManager.MenuSoundType GetSoundType()
    {
        return UIManager.MenuSoundType.useFocused;
    }

    public bool IsSelected()
    {
        return selected;
    }

    public void SetSelected(bool isSelected)
    {
        selected = isSelected;
        menuItemText.fontSize = isSelected ? (int) (initialFontSize * FONT_SIZE_FACTOR) : initialFontSize;
    }

    public override sealed void UseFocused()
    {
        SetSelected(!selected);
        OnIsUsed();
    }

    protected abstract void OnIsUsed();

}