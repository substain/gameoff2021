using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StartSceneMenuItem : SingleMenuItem
{
    [SerializeField]
    private GameScene sceneToStart;

    public override UIManager.MenuSoundType GetSoundType()
    {
        return UIManager.MenuSoundType.useFocused;
    }

    public override void UseFocused()
    {
        MainManager.Instance.LoadScene(sceneToStart);
    }
}