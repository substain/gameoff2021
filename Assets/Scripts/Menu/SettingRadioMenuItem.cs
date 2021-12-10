using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SettingRadioMenuItem : RadioMenuItem
{
    protected override void InitSelection()
    {
        SettingsManager.DifficultySetting currentSetting = SettingsManager.GetDifficulty();
        foreach(SelectableMenuItem item in childrenItems)
        {
            SettingMenuItem settingItem = (SettingMenuItem)item;
            bool isSelected = settingItem.GetSettingType() == currentSetting;
            item.SetSelected(isSelected);
        }
    }
}
