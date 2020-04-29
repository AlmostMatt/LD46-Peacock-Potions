using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance;

    public bool MusicEnabled = true;
    public Sprite onImage;
    public Sprite offImage;
    public GameObject musicButton;

    private bool mPrevMusicEnabled;

    void Start()
    {
        instance = this;
        MusicEnabled = DebugOverrides.MusicEnabled;
        if (MusicEnabled)
        {
            GetComponent<AudioSource>().Play();
        }
    }

    void Update()
    {
        if(mPrevMusicEnabled != MusicEnabled)
        {
            musicButton.GetComponent<Image>().sprite = MusicEnabled ? onImage : offImage;
            mPrevMusicEnabled = MusicEnabled;
        }
    }

    public void ToggleMusic()
    {
        MusicEnabled = !MusicEnabled;
        if (MusicEnabled)
            GetComponent<AudioSource>().UnPause();
        else
            GetComponent<AudioSource>().Pause();
    }

    public static void Stop()
    {
        instance.GetComponent<AudioSource>().Stop();
    }

    public static void Play()
    {
        instance.GetComponent<AudioSource>().Play();
    }
}