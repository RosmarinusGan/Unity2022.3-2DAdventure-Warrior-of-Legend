using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//发布者与订阅者的通信桥梁,发布者调用委托传参,订阅者利用参数处理
[CreateAssetMenu(menuName = "EventDele/FloatDeleSO")]
public class FloatDeleSO : ScriptableObject
{
    public UnityAction<float> OnEventDele;
    public void ActiveDele(float param) => OnEventDele?.Invoke(param);
}
