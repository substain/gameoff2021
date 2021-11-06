using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectScript : MonoBehaviour {

    public string[] preLoadNames;
    public AudioSource audioSource;
    private Dictionary<string, AudioClip> loaded = new Dictionary<string, AudioClip>();

    // Start is called before the first frame update
    void Start() {

        GlobalSound.Register(this);
        audioSource = GetComponent<AudioSource>();

        foreach(string whichSound in preLoadNames) {
            Load(whichSound);
        }
    }

    public void Play(string whichSound) {
        audioSource.PlayOneShot(loaded[whichSound]);
    }

    public void Load(string whichSound) {
        var audioClip = Resources.Load<AudioClip>("Audio/" + whichSound);
        Debug.Log(audioClip);
        if (audioClip == null) {
            Debug.Log("Failed to load audio ''" + whichSound+ "''" );
        } else {
            loaded.Add(whichSound, audioClip);
        }
    }


    void OnDestroy() {
        GlobalSound.Register(this);
    }
}
