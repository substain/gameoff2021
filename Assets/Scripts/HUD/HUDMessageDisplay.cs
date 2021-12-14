using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDMessageDisplay : MonoBehaviour
{
    public const float MESSAGE_DISPLAY_DURATION = 1.5f;
    protected const float MESSAGE_KEEP_SHOWING_TIME = 0.5f;

    [SerializeField]
    protected Text messageText;
    private Timer displayTimer;
    private bool currentIsListenBug;
    private bool isTyping = false;
    private string currentMessage;
    private float relTimePerLetter = 0;
    private bool autoHideAfterTimer = true;
    protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        displayTimer = gameObject.AddComponent<Timer>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    protected virtual void Start()
    {
        displayTimer.SetFixedUpdate(true);
        canvasGroup.alpha = 0;
    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {
        if (isTyping)
        {
            UpdateTyping();
        }
    }

    public void UpdateTyping()
    {
        float timePassed = displayTimer.GetTimePassed();
        float fullDuration = displayTimer.GetFullDuration();
        float typeDuration = fullDuration - MESSAGE_KEEP_SHOWING_TIME;
        float relativeProgress;
        if (timePassed < typeDuration)
        {
            relativeProgress = Mathf.InverseLerp(0, typeDuration, timePassed);
            int lettersShown = Mathf.Min(Mathf.CeilToInt(relativeProgress / relTimePerLetter), currentMessage.Length);
            if (lettersShown <= 0)
            {
                messageText.text = "";
            }
            else
            {
                messageText.text = currentMessage.Substring(0, lettersShown);
            }
        }
        else
        {
            messageText.text = currentMessage;
        }
    }

    public void Display(string message, 
        float duration = MESSAGE_DISPLAY_DURATION, 
        bool isPriority = false, 
        bool useTyping = true,
        bool autoHideAfterTimer = true)
    {
        canvasGroup.alpha = 1;

        this.autoHideAfterTimer = autoHideAfterTimer;

        if ((currentIsListenBug && !isPriority) || message.Length == 0)
        {
            return;
        }

        this.currentIsListenBug = isPriority;
        isTyping = useTyping;
        displayTimer.Init(duration, HideAfterTimer);
        currentMessage = message;

        float typeDuration = duration - MESSAGE_KEEP_SHOWING_TIME;
        if (!isTyping || typeDuration <= 0.05f)
        {
            messageText.text = message;
            isTyping = false;
            return;
        }
        relTimePerLetter = 1 / (float) message.Length;
        messageText.text = "";
    }

    public void HideAfterTimer()
    {
        currentIsListenBug = false;
        isTyping = false;
        currentMessage = "";
        if (!autoHideAfterTimer)
        {
            return;
        }
        messageText.text = "";
        canvasGroup.alpha = 0;
    }

    public void Hide()
    {
        currentIsListenBug = false;
        isTyping = false; 
        currentMessage = "";
        messageText.text = "";
        canvasGroup.alpha = 0;
    }

    public void DisplaySkipped(string content, float timePassed, float fullDuration, bool isPriority, bool showFull)
    {
        float timeLeft = fullDuration - timePassed;
        if (showFull)
        {
            Display(content, isPriority: isPriority);
            return;
        }
        float relativeProgress = Mathf.InverseLerp(0, fullDuration, timePassed);
        int currentLetter = Mathf.FloorToInt(content.Length * relativeProgress);
        if (currentLetter == content.Length - 1)
        {
            return;
        }
        Display(content.Substring(currentLetter, content.Length-currentLetter), timeLeft, isPriority);
    }

    public bool IsTyping()
    {
        return isTyping;
    }

    public void FinishTyping()
    {
        messageText.text = currentMessage;
        currentIsListenBug = false;
        isTyping = false;
        currentMessage = "";
    }

}
