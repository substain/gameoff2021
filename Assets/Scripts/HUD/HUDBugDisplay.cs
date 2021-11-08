using UnityEngine;
using UnityEngine.UI;

public class HUDBugDisplay : MonoBehaviour
{
    private Text bugText;

    void Awake()
    {
        bugText = GetComponentInChildren<Text>();
    }

    public void SetNumberOfBugs(int numBugs)
    {
        bugText.text = "Bugs: " + numBugs;
    }
}
