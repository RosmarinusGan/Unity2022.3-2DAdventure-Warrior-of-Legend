using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//发布者与订阅者的通信桥梁,发布者调用委托传参,订阅者利用参数处理
[CreateAssetMenu(menuName = "EventDele/AudioDeleSO")]
public class AudioDeleSO : ScriptableObject
{
    public UnityAction<AudioClip> OnEventDele;
    public void ActiveDele(AudioClip audioClip) => OnEventDele?.Invoke(audioClip);
}
