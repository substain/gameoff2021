using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUDBugDisplay : MonoBehaviour
{
    public enum BugState
    {
        bugEmpty, bugAvailable, bugPlaced, bugSending
    }

    [SerializeField]
    private List<Sprite> bugStateSprites;
    
    private Image[] bugStatesSpriteRenderer;

    void Awake()
    {
        bugStatesSpriteRenderer = GetComponentsInChildren<Image>(includeInactive:true)
            .OrderBy(sr => sr.transform.position.x)
            .ToArray();
    }

    public void SetBugStates(BugState[] bugStates)
    {
        for(int i = 0; i < bugStates.Length; i++)
        {
            int bugSpriteIndex = (int) bugStates[i];
            bugStatesSpriteRenderer[i].sprite = bugStateSprites[bugSpriteIndex];
        }
    }
}
