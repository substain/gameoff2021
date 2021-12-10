using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenuItem : SelectableMenuItem
{
    [SerializeField]
    SettingsManager.DifficultySetting difficultySetting;

    protected override void OnIsUsed()
    {
        SettingsManager.SetDifficulty(difficultySetting);
    }

    public SettingsManager.DifficultySetting GetSettingType()
    {
        return difficultySetting;
    }
}
