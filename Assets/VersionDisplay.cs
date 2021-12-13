using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionDisplay : MonoBehaviour
{
    private Text versionText;
    void Awake()
    {
        versionText = GetComponentInChildren<Text>();
    }

    void Start()
    {
        //display the version. This can be set in the Unity Editor via Edit -> Project Settings-> Player -> Version
        versionText.text = "Version " + Application.version;
    }
}
