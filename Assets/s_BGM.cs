using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2.System.Core;

public class s_BGM : s_singleton<s_BGM>
{
    public AudioClip music;
    public AudioSource src;

    public void PlaySong(string nameOfSong) {
        AudioClip clp = s_soundmanager.GetInstance().GetClip(nameOfSong);
        if (music == clp)
            return;
        src.loop = true;
        src.clip = clp;
        src.Play();
    }

    public void StopSong() {
        music = null;
        src.Stop();
    }
}
