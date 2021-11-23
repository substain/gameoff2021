using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class IngameOverlayMenu : MonoBehaviour, ISelectableMenu
{
    public enum IngameMenuType
    {
        pause, gameover, options
    }

    [SerializeField]
    private IngameMenuType menuType;

    private IngameMenuType? parent = null;

    private int selectedButtonIndex = 0;
    private List<IngameButton> childrenButtons;

    void Awake()
    {
        //order children buttons by y value
        childrenButtons = GetComponentsInChildren<IngameButton>().OrderByDescending(bs => bs.transform.position.y).ToList();

        //assign them indices
        for(int i = 0; i < childrenButtons.Count; i++)
        {
            childrenButtons[i].SetIndex(i);
        }

        //no buttons
        if (childrenButtons.Count <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void UseSelectedButton()
    {
        childrenButtons[selectedButtonIndex].Activate();
        UseNavigationTarget(childrenButtons[selectedButtonIndex].GetNavigationTarget());
    }

    public void SetIndexSelected(int index)
    {
        selectedButtonIndex = Mathf.Clamp(index, 0, childrenButtons.Count - 1);
        UpdateButtonSelections();
    }

    public void UseBack()
    {
        UseNavigationTarget(MenuNavigationTarget.Back);
    }

    public void UseExit()
    {
        if (parent == IngameMenuType.pause)
        {
            UseNavigationTarget(MenuNavigationTarget.HideMenu);
        }
        UseBack();
    }

    public void SelectNextButton()
    {
        SetIndexSelected(selectedButtonIndex + 1);
    }

    public void SelectPreviousButton()
    {
        SetIndexSelected(selectedButtonIndex - 1);
    }

    private void UpdateButtonSelections()
    {
        for (int i = 0; i < childrenButtons.Count; i++)
        {
            if (i == selectedButtonIndex)
            {
                childrenButtons[i].Select();
            }
            else
            {
                childrenButtons[i].Deselect();
            }
        }
    }

    public IngameMenuType GetMenuType()
    {
        return menuType;
    }

    public void SetParent(IngameMenuType parent)
    {
        this.parent = parent;
    }

    private void UseNavigationTarget(MenuNavigationTarget navTarget)
    {
        switch (navTarget)
        {
            case MenuNavigationTarget.Retry:
                {
                    GameManager.Instance.ReloadCurrentScene();
                    return;
                }
            case MenuNavigationTarget.Options:
                {
                    HUDManager.Instance.ShowIngameMenu(IngameMenuType.options, menuType);
                    return;
                }
            case MenuNavigationTarget.MainMenu:
                {
                    GameManager.Instance.LoadScene(GameScene.mainMenu);
                    return;
                }
            case MenuNavigationTarget.Quit:
                {
                    GameManager.QuitGame();
                    return;
                }
            case MenuNavigationTarget.Back:
                {
                    if(parent != null)
                    {
                        HUDManager.Instance.ShowIngameMenu(parent.Value);
                        return;
                    }

                    if (menuType == IngameMenuType.gameover)
                    {
                        UseNavigationTarget(MenuNavigationTarget.Quit);
                        return;
                    }

                    if (menuType == IngameMenuType.pause)
                    {
                        UseNavigationTarget(MenuNavigationTarget.HideMenu);
                        return;
                    }

                    break;
                }
            case MenuNavigationTarget.HideMenu:
                {
                    HUDManager.Instance.HideIngameMenu();
                    return;
                }
        }
        Debug.LogWarning("could not determine a navigation function for " + navTarget.ToString() + " on a " + menuType.ToString() + " menu.");
    }
}