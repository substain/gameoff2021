using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUDKeyDisplay : MonoBehaviour
{
    private Image[] keySprites;

    void Awake()
    {
        keySprites = GetComponentsInChildren<Image>(includeInactive: true)
            .OrderByDescending(sr => sr.transform.position.x)
            .ToArray();
    }

    public void SetObtainedKeys(HashSet<int> obtainedKeyIds)
    {
        if (obtainedKeyIds.Count <= 0)
        {
            return;
        }
        if(obtainedKeyIds.Count > keySprites.Length)
        {
            int amountToReduce = obtainedKeyIds.Count- keySprites.Length;
            Debug.LogWarning("The player has more keys than UI items.");
        }
        int index = 0;
        foreach (int keyId in obtainedKeyIds)
        {
            keySprites[index].gameObject.SetActive(true);
            keySprites[index].color = KeyIdToColor(keyId);
            index++; 
        }
        for(int i = index; i < keySprites.Length; i++)
        {
            keySprites[index].gameObject.SetActive(false);
        }
    }

    public static string KeyIdToName(int keyId)
    {
        switch (keyId)
        {
            case 1:
                {
                    return "Red";
                }
            case 2:
                {
                    return "Green";
                }
            default:
                {
                    Debug.LogWarning("Key with id " + keyId + " does not have a name.");
                    return "? Key";
                }
        }
    }

    public static Color KeyIdToColor(int keyId)
    {
        float keyColor = ((float)keyId) / 16;

        return Color.HSVToRGB(keyColor, 1, 1);
    }
}
