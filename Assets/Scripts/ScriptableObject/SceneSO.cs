using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

//实例以序列化资源的形式保存场景信息
[CreateAssetMenu(menuName = "SceneSO")]
public class SceneSO : ScriptableObject
{
    [SerializeField]public SceneType sceneType; //这里为什么要用序列化装饰？
    public AssetReference sceneAsset;
}
