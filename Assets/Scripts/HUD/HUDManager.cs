using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static IngameOverlayMenu;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance = null;
    private HUDActionHint hudActionHint;
    private HUDMessageDisplay hudMessageDisplay;
    private HUDBugDisplay hudBugDisplay;
    private HUDKeyDisplay hudKeyDisplay;
    private HUDListenBugDisplay hudListenBugDisplay;
    private HUDDialogueDisplay hudDialogueDisplay;

    private List<IngameOverlayMenu> ingameMenus;
    private IngameOverlayMenu activeMenu = null;

    public delegate void IngameMenuClosed();
    public static event IngameMenuClosed OnCloseIngameMenu;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("There is more than one HUDManager in this scene.");
        }
        Instance = this;
        hudActionHint = GetComponentInChildren<HUDActionHint>();
        hudMessageDisplay = GetComponentInChildren<HUDMessageDisplay>();
        hudBugDisplay = GetComponentInChildren<HUDBugDisplay>();
        hudKeyDisplay = GetComponentInChildren<HUDKeyDisplay>();
        hudListenBugDisplay = GetComponentInChildren<HUDListenBugDisplay>();
        hudDialogueDisplay = GetComponentInChildren<HUDDialogueDisplay>();

        ingameMenus = new List<IngameOverlayMenu>(GetComponentsInChildren<IngameOverlayMenu>());
    }

    void Start()
    {
        HideIngameMenu();
    }

    public void UpdateActionHintText(string hintText)
    {
        hudActionHint.SetActionHint(hintText);
    }
    
    public void DisplayMessage(string message)
    {
        hudMessageDisplay.Display(message);
    }

    public void SetNumberOfBugs(int numBugs)
    {
        hudBugDisplay.SetNumberOfBugs(numBugs);
    }

    public void SetObtainedKeys(List<int> obtainedKeyIds)
    {
        hudKeyDisplay.SetObtainedKeys(obtainedKeyIds);
    }

    public void SetCurrentActiveBugId(int bugId)
    {
        hudListenBugDisplay.SetActiveBugId(bugId);
    }

    public void ShowDialog(DialogueLine dialogueLine, Transform playerPosition, Transform targetPosition)
    {
        hudDialogueDisplay.ShowDialogue(dialogueLine, playerPosition, targetPosition);
    }

    public void CloseDialogue()
    {
        hudDialogueDisplay.CloseDialogue();
    }

    public void IngameMenuUseExit()
    {
        activeMenu.UseBack();
    }
    
    public void IngameMenuUseBack()
    {
        activeMenu.UseBack();
    }

    public void IngameMenuSelectNext()
    {
        activeMenu.SelectNextButton();
    }

    public void IngameMenuSelectPrevious()
    {
        activeMenu.SelectPreviousButton();
    }

    public void IngameMenuUseSelected()
    {
        activeMenu.UseSelectedButton();
    }

    public void ShowIngameMenu(IngameMenuType menuType, IngameMenuType? parentType = null)
    {
        foreach(IngameOverlayMenu iom in ingameMenus)
        {
            if(iom.GetMenuType() == menuType)
            {
                iom.gameObject.SetActive(true);
                activeMenu = iom;
                if (parentType.HasValue)
                {
                    iom.SetParent(parentType.Value);
                }
            }
            else
            {
                iom.gameObject.SetActive(false);
            }
        }
    }

    public void HideIngameMenu()
    {
        activeMenu = null;
        foreach (IngameOverlayMenu iom in ingameMenus)
        {
            iom.gameObject.SetActive(false);
        }
        OnCloseIngameMenu?.Invoke();
    }

    void OnDestroy()
    {
        Instance = null;
    }
}