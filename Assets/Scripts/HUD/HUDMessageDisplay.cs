using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDMessageDisplay : MonoBehaviour
{
    private const float MESSAGE_DISPLAY_DURATION = 1.5f;
    private Text displayText;
    private Timer displayTimer;
    private bool isShowingContent = false;

    void Awake()
    {
        displayTimer = gameObject.AddComponent<Timer>();
        displayText = GetComponentInChildren<Text>();
    }

    public void Display(string message, float duration = MESSAGE_DISPLAY_DURATION)
    {
        if (isShowingContent)
        {
            Debug.Log("HUDMessageDisplay: did not display simple message because you were showing content");
            return;
        }
        displayText.text = message;
        displayTimer.Init(duration, Hide);
    }

    public void Hide()
    {
        displayText.text = "";
    }

    public void StartShowingContent(string listenContent, float currentProgress, float fullDuration)
    {
        Debug.Log("start showing content called. not implemented. showing as simple message for " + (fullDuration - currentProgress));
        Display(listenContent, fullDuration - currentProgress);
        isShowingContent = true;
    }

    public void StopShowingContent()
    {
        isShowingContent = false;
        Hide();
    }

}
