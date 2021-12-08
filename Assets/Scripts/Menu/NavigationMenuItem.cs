using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class NavigationMenuItem : SingleMenuItem
{
    [SerializeField]
    MenuNavigationTarget menuNavigationTarget;

    public override UIManager.MenuSoundType GetSoundType()
    {
        if(menuNavigationTarget == MenuNavigationTarget.Parent)
        {
            return UIManager.MenuSoundType.back;
        } else if (menuNavigationTarget == MenuNavigationTarget.HideMenu)
        {
            return UIManager.MenuSoundType.closeMenu;
        }
        return UIManager.MenuSoundType.useFocused;
    }

    public override void UseFocused()
    {
        menuController.UseNavigationTarget(menuNavigationTarget);
    }
}
