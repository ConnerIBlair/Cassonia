using System.Collections;
using UnityEngine;

public class VolumeScript : MonoBehaviour
{
    public float masterVolume;
    public float effectVolume;
    public float songVolume;

    public AudioSource effectPlayer;
    public AudioSource songPlayer;

    private float volumeStorage;
    public AudioClip nextSong;

    private static VolumeScript instance = null;

    public static VolumeScript Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);

        if (masterVolume == .999f)
        {
            PlayerPrefs.SetFloat("mVolume", masterVolume);
            PlayerPrefs.SetFloat("sVolume", songVolume);
            PlayerPrefs.SetFloat("eVolume", effectVolume);
        }
        masterVolume = PlayerPrefs.GetFloat("mVolume");
        songVolume = PlayerPrefs.GetFloat("sVolume");
        effectVolume = PlayerPrefs.GetFloat("eVolume");

        songPlayer.volume = masterVolume * songVolume;
        effectPlayer.volume = masterVolume * effectVolume;

    }

    public void ChangeVolume(float v, int type)
    {
        if (type == 0)
        {
            masterVolume = v;
            PlayerPrefs.SetFloat("mVolume", v);
            songPlayer.volume = v * songVolume;
            effectPlayer.volume = v * effectVolume;
        }
        if (type == 1)
        {
            songVolume = v;
            PlayerPrefs.SetFloat("sVolume", v);
            songPlayer.volume = v * masterVolume;
        }
        if (type == 2)
        {
            effectVolume = v;
            PlayerPrefs.SetFloat("eVolume", v);
            effectPlayer.volume = v * masterVolume;
        }
    }

    public void PlayEffect(AudioClip effectToPlay, float volume = 1)
    {
        effectPlayer.PlayOneShot(effectToPlay, volume);
    }

    public void PlaySong(AudioClip songToPlay, bool skipFadeOut, bool skipFadeIn = false)
    {
        if (songToPlay != songPlayer.clip)
        {
            volumeStorage = PlayerPrefs.GetFloat("sVolume");
            StartCoroutine(FadeOutCo(2, skipFadeOut, skipFadeIn));
            nextSong = songToPlay;
        }
    }

    private IEnumerator FadeOutCo(int fadeTime, bool skip, bool skipIn)
    {
        if (songVolume == 0 || skip == true)
        {
            songPlayer.clip = nextSong;
            songPlayer.Play();
            yield break;
        }
        for (float i = songVolume; i > 0; i -= .02f / fadeTime * volumeStorage)
        {
            songPlayer.volume = i;
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(FadeInCo(3, skipIn));
    }

    private IEnumerator FadeInCo(int fadeTime, bool skip)
    {
        yield return new WaitForSeconds(1);
        songPlayer.clip = nextSong;
        songPlayer.Play();
        if (volumeStorage == 0) // Fail safe if volume is 0
        {
            yield break;
        }
        print(songVolume);
        if (skip)
        {
            songPlayer.volume = volumeStorage;
            yield break;
        }
        for (float i = 0; i < volumeStorage; i += .02f / fadeTime * volumeStorage)
        {
            songPlayer.volume = i;
            yield return new WaitForFixedUpdate();
        }
    }
}
