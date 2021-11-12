using UnityEngine;
using UnityEngine.UI;

public class HUDListenBugDisplay : MonoBehaviour
{
    private Text activeBugText;

    void Awake()
    {
        activeBugText = GetComponentInChildren<Text>();
    }

    public void SetActiveBugId(int bugId)
    {
        string bugName = bugId == -1 ? "None" : bugId.ToString();
        activeBugText.text = "Active Bug: " + bugName;
    }
}