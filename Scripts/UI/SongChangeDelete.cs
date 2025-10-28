using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongChangeDelete : MonoBehaviour
{
    public VolumeScript cScript;

    public AudioClip song;

    //private void Start()
    //{
    //    cScript = FindObjectOfType<VolumeScript>();
    //    cScript.nextSong = song;
    //}
    public void changeSong(AudioClip songToPlay)
    {
        cScript = FindFirstObjectByType<VolumeScript>();
        cScript.nextSong = song;
        cScript.PlaySong(songToPlay, false);
    }
}
