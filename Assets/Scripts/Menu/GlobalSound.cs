using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalSound  {

    private static SoundEffectScript soundPlayer;
    
    public static void Play(string whichSound) {
        soundPlayer.Play(whichSound);
    }

    public static void Load(string whichSound) {
        soundPlayer.Load(whichSound);
    }

    public static void Register(SoundEffectScript SE_script) {
        if (soundPlayer != null) {
            Debug.Log("Multiple SoundEffectScript loaded at once - older one in use");
            return;
        } else {
            soundPlayer = SE_script;
        }
    }

    public static void Unregister(SoundEffectScript SE_script) {
        soundPlayer = null;
    }
}
