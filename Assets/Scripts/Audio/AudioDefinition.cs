using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//播放audio事件发布
public class AudioDefinition : MonoBehaviour
{
    public AudioDeleSO audioDele;
    public AudioClip audioClip;
    public bool playOnEnable;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            ActiveAudioDele();
        }
    }

    public void ActiveAudioDele() => audioDele.ActiveDele(audioClip);
}
