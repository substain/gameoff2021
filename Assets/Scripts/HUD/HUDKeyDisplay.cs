using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUDKeyDisplay : MonoBehaviour
{
    private Text keyText;

    void Awake()
    {
        keyText = GetComponentInChildren<Text>();
    }

    public void SetObtainedKeys(List<int> obtainedKeyIds)
    {
        string keysString;
        if(obtainedKeyIds.Count > 0)
        {
            keysString = string.Join(", ", obtainedKeyIds.Select(keyId => KeyIdToName(keyId)));
        }
        else
        {
            keysString = "None";
        }

        keyText.text = "Keys: " + keysString;
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
}
