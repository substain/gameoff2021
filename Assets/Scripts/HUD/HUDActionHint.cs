using UnityEngine;
using UnityEngine.UI;

public class HUDActionHint : MonoBehaviour
{
    private Text actionHintText;

    void Awake()
    {
        actionHintText = GetComponentInChildren<Text>();
    }

    public void SetActionHint(string text)
    {
        actionHintText.text = text;
    }
}
