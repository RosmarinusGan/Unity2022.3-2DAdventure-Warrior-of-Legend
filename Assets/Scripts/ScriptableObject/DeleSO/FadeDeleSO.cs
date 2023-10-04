using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//发布者与订阅者的通信桥梁,发布者调用委托传参,订阅者利用参数处理
[CreateAssetMenu(menuName = "EventDele/FadeDeleSO")]
public class FadeDeleSO : ScriptableObject
{
    public UnityAction<Color, float> OnEventDele;

    public void ActiveFadeIn(float fadeDuration) => OnEventDele?.Invoke(Color.black, fadeDuration);
    public void ActiveFadeOut(float fadeDuration) => OnEventDele?.Invoke(Color.clear, fadeDuration);
}
