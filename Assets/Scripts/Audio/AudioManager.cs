using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public AudioSource BGMSource, FXSource;
    public AudioMixer mainMixer;
    // AudioManager作为发布者
    public UpdateSliderDeleSO UpdateSliderDeleSo;
    
    // AudioManager作为订阅者
    public AudioDeleSO FXAudioDeleSo, BGMAudioDeleSo;
    public FloatDeleSO MasterVolumeChangeDeleSo, BGMVolumeChangeDeleSo, FXVolumeChangeDeleSo;
    public VoidDeleSO GamePauseDeleSo;

    private void OnEnable()
    {
        FXAudioDeleSo.OnEventDele += SetFXAudioClip;
        BGMAudioDeleSo.OnEventDele += SetBGMAudioClip;
        MasterVolumeChangeDeleSo.OnEventDele += OnMasterVolumeChangeAction;
        BGMVolumeChangeDeleSo.OnEventDele += OnBGMVolumeChangeAction;
        FXVolumeChangeDeleSo.OnEventDele += OnFXVolumeChangeAction;
        GamePauseDeleSo.OnEventDele += OnGamePauseAction;
    }
    
    private void OnDisable()
    {
        FXAudioDeleSo.OnEventDele -= SetFXAudioClip;
        BGMAudioDeleSo.OnEventDele -= SetBGMAudioClip;
        MasterVolumeChangeDeleSo.OnEventDele -= OnMasterVolumeChangeAction;
        BGMVolumeChangeDeleSo.OnEventDele -= OnBGMVolumeChangeAction;
        FXVolumeChangeDeleSo.OnEventDele -= OnFXVolumeChangeAction;
        GamePauseDeleSo.OnEventDele -= OnGamePauseAction;
    }

    private void SetFXAudioClip(AudioClip audioClip)
    {
        FXSource.clip = audioClip;
        FXSource.Play();
    }

    private void SetBGMAudioClip(AudioClip audioClip)
    {
        BGMSource.clip = audioClip;
        BGMSource.Play();
    }
    
    private void OnGamePauseAction()
    {
        mainMixer.GetFloat("MasterVolume", out var master);
        UpdateSliderDeleSo.ActiveDele(master, 1);
        mainMixer.GetFloat("BGMVolume", out var bgm);
        UpdateSliderDeleSo.ActiveDele(bgm, 2);
        mainMixer.GetFloat("FXVolume", out var fx);
        UpdateSliderDeleSo.ActiveDele(fx, 3);
    }
    
    private void OnMasterVolumeChangeAction(float volume) => mainMixer.SetFloat("MasterVolume", volume * 100 - 80);
    private void OnBGMVolumeChangeAction(float volume) => mainMixer.SetFloat("BGMVolume", volume * 100 - 80);
    private void OnFXVolumeChangeAction(float volume) => mainMixer.SetFloat("FXVolume", volume * 100 - 80);
}
