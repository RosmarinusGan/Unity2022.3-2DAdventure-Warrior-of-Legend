using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//发布者与订阅者的通信桥梁,发布者调用委托传参,订阅者利用参数处理
[CreateAssetMenu(menuName = "EventDele/SceneSwitchDeleSO")]
public class SceneSwitchDeleSO : ScriptableObject
{
    public UnityAction<SceneSO, Vector3, bool> OnEventDele;
    
    /// <summary>
    /// 委托激活
    /// </summary>
    /// <param name="targetScene">目标加载场景</param>
    /// <param name="targetPosition">玩家位置</param>
    /// <param name="isFade">淡入淡出</param>
    public void ActiveDele(SceneSO targetScene, Vector3 targetPosition, bool isFade) =>
        OnEventDele?.Invoke(targetScene, targetPosition, isFade);
}
