using UnityEngine;
using UnityEngine.UI;

public class HUDActionHint : MonoBehaviour
{
    [SerializeField]
    private bool isEnabled = true;

    private Text actionHintText;

    void Awake()
    {
        actionHintText = GetComponentInChildren<Text>();
        this.actionHintText.color = isEnabled ? actionHintText.color : Color.clear;
    }

    public void SetActionHint(string text)
    {
        actionHintText.text = text;
    }
}
