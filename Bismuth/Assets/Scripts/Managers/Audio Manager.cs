using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // SFX(효과음) 재생
    public void PlayBGM(AudioSource source, float volume = 1.0f, float pitch = 1.0f)
    {
        if (source == null) return;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
    }

    public void PlaySFX(AudioSource source, float volume = 1.0f, float pitch = 1.0f)
    {
        if (source == null) return;
        source.volume = volume;
        source.pitch = pitch;
        source.PlayOneShot(source.clip);
    }
}