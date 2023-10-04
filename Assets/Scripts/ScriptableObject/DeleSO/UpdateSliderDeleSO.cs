using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//发布者与订阅者的通信桥梁,发布者调用委托传参,订阅者利用参数处理
[CreateAssetMenu(menuName = "EventDele/UpdateSliderDeleSO")]
public class UpdateSliderDeleSO : ScriptableObject
{
    public UnityAction<float, int> OnEventDele;
    public void ActiveDele(float param, int index) => OnEventDele?.Invoke(param, index);
}
