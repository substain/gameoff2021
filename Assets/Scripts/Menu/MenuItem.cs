using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class MenuItem : MonoBehaviour
{
    protected MenuController menuController;

    public void SetMenuController(MenuController menuController)
    {
        this.menuController = menuController;
    }

    public abstract void SetFocused(bool isFocused);

    public abstract void UseFocused();

    public abstract void SelectNext();

    public abstract void SelectPrevious();

    public abstract UIManager.MenuSoundType GetSoundType();
}
