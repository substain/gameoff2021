using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance = null;
    private HUDActionHint hudActionHint;
    private HUDMessageDisplay hudMessageDisplay;
    private HUDBugDisplay hudBugDisplay;
    private HUDKeyDisplay hudKeyDisplay;

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
}