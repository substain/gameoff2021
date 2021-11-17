using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUDDialogueDisplay : MonoBehaviour
{
    private const float FADEOUT_DISPLAY_RANGE = 4;
    private const float MAX_DISPLAY_RANGE = 8;

    private Text displayText;
    private CanvasGroup canvasGroup;
    private Transform refPosition;
    private Transform targetPosition = null;
    private bool hasActiveDialogue;
    void Awake()
    {
        hasActiveDialogue = false;
        canvasGroup = GetComponent<CanvasGroup>();
        displayText = GetComponentInChildren<Text>();
    }

    void Update()
    {
        if (hasActiveDialogue && refPosition != null && targetPosition != null)
        {
            ComputeAlphaByDistance();
        }
    }

    private void ComputeAlphaByDistance()
    {
        float dist = Vector3.Distance(refPosition.position, targetPosition.position);
        float alphaByDist = 1 - Mathf.Clamp01(dist - FADEOUT_DISPLAY_RANGE / MAX_DISPLAY_RANGE - FADEOUT_DISPLAY_RANGE);
        canvasGroup.alpha = alphaByDist;
    }

    public void ShowDialogue(DialogueLine dialogueLine, Transform playerPosition, Transform targetPosition)
    {
        this.refPosition = playerPosition;
        this.targetPosition = targetPosition;
        hasActiveDialogue = true;
        displayText.text = dialogueLine.GetMergedLine();
    }

    public void CloseDialogue()
    {
        hasActiveDialogue = false;
        displayText.text = "";
        canvasGroup.alpha = 0;
    }
}
