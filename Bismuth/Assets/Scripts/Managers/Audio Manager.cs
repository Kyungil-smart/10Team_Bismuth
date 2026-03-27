using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private float _masterVolume = 1f;  // 전체
    private float _bgmVolume = 1f;     // 배경음악
    private float _sfxVolume = 1f;     // 전투효과음
    private float _uiVolume = 1f;      // UI 효과음

    public float MasterVolume => _masterVolume;
    public float BgmVolume => _bgmVolume;
    public float SfxVolume => _sfxVolume;
    public float UIVolume => _uiVolume;

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

    // BGM 재생
    public void PlayBGM(AudioSource source, float volume = 1.0f, float pitch = 1.0f)
    {
        if (source == null) return;
        source.volume = volume * _bgmVolume * _masterVolume;
        source.pitch = pitch;
        source.Play();
    }

    // 전투 효과음 재생
    public void PlaySFX(AudioSource source, float volume = 1.0f, float pitch = 1.0f)
    {
        if (source == null) return;
        source.volume = volume * _sfxVolume * _masterVolume;
        source.pitch = pitch;
        source.PlayOneShot(source.clip);
    }

    // UI 효과음 재생
    public void PlayUI(AudioSource source, float volume = 1.0f, float pitch = 1.0f)
    {
        if (source == null) return;
        source.volume = volume * _uiVolume * _masterVolume;
        source.pitch = pitch;
        source.PlayOneShot(source.clip);
    }

    public void SetMasterVolume(float value)
    {
        _masterVolume = value;
    }
    
    public void SetBgmVolume(float value)
    {
        _bgmVolume = value;
    }

    public void SetSfxVolume(float value)
    {
        _sfxVolume = value;
    }

    public void SetUIVolume(float value)
    {
        _uiVolume = value;
    }
}