using System.Collections.Generic;
using UnityEngine;

public class HUDManager : UIManager
{
    [SerializeField]
    private bool actionHintEnabled = false;

    public static HUDManager HUDInstance = null;

    private HUDActionHint hudActionHint;
    private HUDMessageDisplay hudMessageDisplay;
    private HUDBugDisplay hudBugDisplay;
    private HUDKeyDisplay hudKeyDisplay;
    private HUDDialogueDisplay hudDialogueDisplay;

    protected override void Awake()
    {
        base.Awake();
        SetInstance();
        hudActionHint = GetComponentInChildren<HUDActionHint>();
        hudMessageDisplay = GetComponentInChildren<HUDMessageDisplay>();
        hudBugDisplay = GetComponentInChildren<HUDBugDisplay>();
        hudKeyDisplay = GetComponentInChildren<HUDKeyDisplay>();
        hudDialogueDisplay = GetComponentInChildren<HUDDialogueDisplay>();
    }

    private void SetInstance()
    {
        if (HUDInstance != null)
        {
            Debug.LogWarning("There is more than one " + this.GetType().ToString() + " in this scene.");
        }
        HUDInstance = this;
    }

    public void UpdateActionHintText(string hintText)
    {
        if (!actionHintEnabled)
        {
            return;
        }
        hudActionHint.SetActionHint(hintText);
    }
    
    public void DisplayMessage(string message, float duration = HUDMessageDisplay.MESSAGE_DISPLAY_DURATION, bool isListenBug = false)
    {
        hudMessageDisplay.Display(message, duration: duration, isPriority: isListenBug);
    }

    public void SetBugStates(HUDBugDisplay.BugState[] bugStates)
    {
        hudBugDisplay.SetBugStates(bugStates);
    }

    public void SetObtainedKeys(HashSet<int> obtainedKeyIds)
    {
        hudKeyDisplay.SetObtainedKeys(obtainedKeyIds);
    }

    public void ShowDialogue(DialogueLine dialogueLine, Transform playerPosition, Transform targetPosition)
    {
        hudDialogueDisplay.ShowDialogue(dialogueLine, playerPosition, targetPosition);
    }

    public void CloseDialogue()
    {
        hudDialogueDisplay.CloseDialogue();
    }

    public void FinishTypingDialogue()
    {
        hudDialogueDisplay.FinishTyping();
    }

    public bool IsTypingDialogue()
    {
        return hudDialogueDisplay.IsTyping();
    }

    public void StopListenContent()
    {
        hudMessageDisplay.Hide();
    }

    public void ShowSkippedContent(string listenContent, float timePassed, float fullDuration, bool isContinuous)
    {
        hudMessageDisplay.DisplaySkipped(listenContent, timePassed, fullDuration, true, isContinuous);
    }

    void OnDestroy()
    {
        HUDInstance = null;
    }
}