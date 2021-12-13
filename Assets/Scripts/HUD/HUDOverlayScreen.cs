using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDOverlayScreen : MonoBehaviour
{

    private bool isRunning = false;
    private Timer timer;
    private CanvasGroup canvasGroup;

    private Text overlayText;

    private float previousValue = 0;
    private float nextValue = 0;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        overlayText = GetComponentInChildren<Text>();
        timer = gameObject.AddComponent<Timer>();
    }

    void Update()
    {
        if (isRunning)
        {
            canvasGroup.alpha = Mathf.Lerp(previousValue, nextValue, timer.GetRelativeProgress());
        }
    }

    public void SetOverlayText(string text)
    {
        overlayText.text = text;
    }

    public void FadeTo(float targetValue, float fadeDuration)
    {
        isRunning = true;
        previousValue = canvasGroup.alpha;
        nextValue = targetValue;

        timer.Init(fadeDuration, Finish);
    }

    public void FadeIn(float fadeDuration)
    {
        FadeTo(1.0f, fadeDuration);
    }

    public void FadeOut(float fadeDuration)
    {
        FadeTo(0.0f, fadeDuration);
    }

    private void Finish()
    {
        canvasGroup.alpha = nextValue;
        isRunning = false;
    }

    public void SetVisible(bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1 : 0;
    }

    public bool IsFading()
    {
        return isRunning;
    }
}