using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDMessageDisplay : MonoBehaviour
{
    private const float MESSAGE_DISPLAY_DURATION = 1.5f;
    private Text displayText;
    private Timer displayTimer;

    void Awake()
    {
        displayTimer = gameObject.AddComponent<Timer>();
        displayText = GetComponentInChildren<Text>();
    }

    public void Display(string message, float duration = MESSAGE_DISPLAY_DURATION)
    {
        displayText.text = message;
        displayTimer.Init(duration, Hide);
    }

    public void Hide()
    {
        displayText.text = "";
    }

}
