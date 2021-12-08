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

    [SerializeField]
    private Text titleText;

    private CanvasGroup canvasGroup;

    private AudioSource menuSfxAudioSource;
    private AudioClip backMenuClip;

    void Awake()
    {
        //order children buttons by y value
        childrenButtons = GetComponentsInChildren<IngameButton>().OrderByDescending(bs => bs.transform.position.y).ToList();

        //assign them indices
        for(int i = 0; i < childrenButtons.Count; i++)
        {
            childrenButtons[i].SetIndex(i);
            childrenButtons[i].GetComponent<Button>().interactable = false;
        }

        //no buttons
        if (childrenButtons.Count <= 0)
        {
            gameObject.SetActive(false);
        }

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public void SetEnabled(bool isEnabled)
    {
        childrenButtons.ForEach(button => button.GetComponent<Button>().interactable = isEnabled);
        canvasGroup.alpha = isEnabled ? 1 : 0;
    }

    private void SetChildrenActive(bool childrenActive)
    {
        foreach (Transform child in transform)
        {
            Debug.Log("child:" + child.name);
            child.gameObject.SetActive(childrenActive);
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
        UseNavigationTarget(MenuNavigationTarget.Parent);
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

    public void SetTitle(string title)
    {
        this.titleText.text = title;
    }

    public void SetParent(IngameMenuType parent)
    {
        this.parent = parent;
    }

    public void SetMenuAudio(AudioSource audioSource, AudioClip selectClip, AudioClip useSelectedClip, AudioClip backMenuClip)
    {
        this.menuSfxAudioSource = audioSource;
        this.backMenuClip = backMenuClip;
        childrenButtons.ForEach(button => {
            button.SetAudioSource(audioSource);
            button.SetAudioClips(selectClip, useSelectedClip);
        });
    }

    private void UseNavigationTarget(MenuNavigationTarget navTarget)
    {
        switch (navTarget)
        {
            case MenuNavigationTarget.RestartScene:
                {
                    GameManager.Instance.ReloadCurrentScene();
                    return;
                }
            case MenuNavigationTarget.Options:
                {
                    //HUDManager.Instance.SetMenuActive(IngameMenuType.options, null, menuType);
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
            case MenuNavigationTarget.Parent:
                {
                    if(parent != null)
                    {
                        menuSfxAudioSource.PlayOneShot(backMenuClip);
                        //HUDManager.Instance.SetMenuActive(parent.Value);
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
                    GameManager.Instance.HideIngameMenu();
                    return;
                }
        }
        Debug.LogWarning("could not determine a navigation function for " + navTarget.ToString() + " on a " + menuType.ToString() + " menu.");
    }

}