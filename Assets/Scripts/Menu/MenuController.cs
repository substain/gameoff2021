using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public enum MenuType
    {
        mainMenu, pause, gameover, options
    }

    [SerializeField]
    private MenuType menuType;

    [SerializeField]
    private bool activeOnStart = false;

    [SerializeField]
    private Text titleText;

    private MenuType? parent = null;
    private List<MenuItem> childrenItems;
    private int focusedItemIndex = 0;

    void Awake()
    {
        //order children items by y value
        childrenItems = GetComponentsInChildren<MenuItem>(includeInactive: true)
            .Where(mi => !mi.GetType().IsSubclassOf(typeof(SelectableMenuItem))) //only direct children
            .OrderByDescending(bs => bs.transform.position.y).ToList();

        childrenItems.ForEach(ci => ci.SetMenuController(this));
    }

    void Start()
    {
        UpdateItemFocus();
    }

    public MenuType GetMenuType()
    {
        return menuType;
    }

    public void SetTitle(string title)
    {
        titleText.text = title;
    }

    public void SetParent(MenuType parent)
    {
        this.parent = parent;
    }

    public void NavigateDirectional(Util.Dir4 dir)
    {
        switch (dir)
        {
            case Util.Dir4.North:
                {
                    FocusPreviousItem();
                    break;
                }
            case Util.Dir4.South:
                {
                    FocusNextItem();
                    break;
                }
            case Util.Dir4.East:
                {
                    FocusNextWithinItem();
                    break;
                }
            case Util.Dir4.West:
                {
                    FocusPreviousWithinItem();
                    break;
                }
        }
    }

    public void FocusNextItem()
    {
        SetIndexFocused(focusedItemIndex + 1);
    }

    public void FocusPreviousItem()
    {
        SetIndexFocused(focusedItemIndex - 1);
    }

    public void FocusNextWithinItem()
    {
        childrenItems[focusedItemIndex].FocusNext();
    }

    public void FocusPreviousWithinItem()
    {
        childrenItems[focusedItemIndex].FocusPrevious();
    }

    public void UseBack()
    {
        UseNavigationTarget(MenuNavigationTarget.Parent);
    }

    public void SetIndexFocused(int index)
    {
        focusedItemIndex = Mathf.Clamp(index, 0, childrenItems.Count - 1);
        UpdateItemFocus();
    }

    private void UpdateItemFocus()
    {
        for (int i = 0; i < childrenItems.Count; i++)
        {
            if (i == focusedItemIndex)
            {
                childrenItems[i].SetFocused(true);
            }
            else
            {
                childrenItems[i].SetFocused(false);
            }
        }
    }

    public UIManager.MenuSoundType GetFocusedSoundType()
    {
        return childrenItems[focusedItemIndex].GetSoundType();
    }

    public void UseFocusedItem()
    {
        childrenItems[focusedItemIndex].UseFocused();
    }

    public bool IsEnabledOnStart()
    {
        return activeOnStart;
    }

    public void SetEnabled(bool isEnabled)
    {
        /*foreach (Transform child in transform)
        {
            Debug.Log("child:" + child.name);
            child.gameObject.SetActive(childrenActive);
        }*/
        //childrenButtons.ForEach(button => button.GetComponent<Button>().interactable = isEnabled);
        //canvasGroup.alpha = isEnabled ? 1 : 0;
    }

    public void UseNavigationTarget(MenuNavigationTarget navTarget)
    {
        switch (navTarget)
        {
            case MenuNavigationTarget.RestartScene:
                {
                    GameManager.GameInstance.ReloadCurrentScene();
                    return;
                }
            case MenuNavigationTarget.Options:
                {
                    UIManager.Instance.SetMenuActive(MenuType.options, menuType);
                    return;
                }
            case MenuNavigationTarget.Parent:
                {
                    if (parent.HasValue)
                    {
                        UIManager.Instance.SetMenuActive(parent.Value);
                        return;
                    }
                    else
                    {
                        Debug.LogWarning(menuType.ToString() + " menu is trying to get back to its parent menu, which is set to null.");
                    }
                    break;
                }
            case MenuNavigationTarget.MainMenu:
                {
                    MainManager.Instance.LoadScene(GameScene.mainMenu);
                    return;
                }
            case MenuNavigationTarget.Quit:
                {
                    MainManager.QuitGame();
                    return;
                }

            case MenuNavigationTarget.HideMenu:
                {
                    GameManager.GameInstance.HideIngameMenu();
                    return;
                }
        }
        Debug.LogWarning("Could not determine a navigation function for " + navTarget.ToString() + " on " + menuType.ToString() + " menu.");
    }
}