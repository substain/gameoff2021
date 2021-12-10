using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class PlayMusicZone : MonoBehaviour
{
    [SerializeField]
    private AudioClip musicToStart;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameManager.GameInstance.StartBackgroundMusicClip(musicToStart);
        }
    }
}