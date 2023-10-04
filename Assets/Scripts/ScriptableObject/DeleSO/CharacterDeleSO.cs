using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//发布者与订阅者的通信桥梁,发布者调用委托传参,订阅者利用参数处理
//发布角色血量变化事件的委托订阅
[CreateAssetMenu(menuName = "EventDele/CharacterDeleSO")]
public class CharacterDeleSO : ScriptableObject
{
    public UnityAction<Character> OnEventDele;
    public void ActiveDele(Character character) => OnEventDele?.Invoke(character);
}
