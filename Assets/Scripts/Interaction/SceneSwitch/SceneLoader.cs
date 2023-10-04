using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SceneLoader : SaveBehaviour
{
    // SceneLoader作为订阅者
    public SceneSwitchDeleSO sceneSwitchDeleSo;
    public VoidDeleSO newGameDeleSo;
    public VoidDeleSO backToMenuDeleSo;
    
    // SceneLoader作为发布者
    public VoidDeleSO afterSceneLoadDeleSo;
    public FadeDeleSO fadeDeleSo;
    public SceneSwitchDeleSO unloadedSceneDeleSo;
    
    public SceneSO initialScene, menuScene;
    public Vector3 initialPos, menuPos;
    public float fadeTime;
    public Transform playerTransform;
    
    private SceneSO currentScene;
    private bool isLoading;
    
    public override void SendData(Data data)
    {
        data.SaveScene(currentScene);
    }

    public override void LoadData(Data data)
    {
        if (data.characterPosDict.ContainsKey(playerTransform.GetComponent<GUIDGenerator>().GUID))
        {
            sceneSwitchDeleSo.ActiveDele(data.LoadScene(), data.characterPosDict[playerTransform.GetComponent<GUIDGenerator>().GUID], true);
        }
    }
    
    private void Start()
    {
        sceneSwitchDeleSo.ActiveDele(menuScene, menuPos, true);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        sceneSwitchDeleSo.OnEventDele += SceneSwitchRequest;
        newGameDeleSo.OnEventDele += InitializeGame;
        backToMenuDeleSo.OnEventDele += OnBackToMenuAction;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        sceneSwitchDeleSo.OnEventDele -= SceneSwitchRequest;
        newGameDeleSo.OnEventDele -= InitializeGame;
        backToMenuDeleSo.OnEventDele -= OnBackToMenuAction;
    }

    private void InitializeGame()
    {
        DataManager.Instance.DeleteData();
        sceneSwitchDeleSo.ActiveDele(initialScene, initialPos, true);
    }

    private void OnBackToMenuAction() => sceneSwitchDeleSo.ActiveDele(menuScene, menuPos, true);
    
    private void SceneSwitchRequest(SceneSO targetScene, Vector3 playerPos, bool isFade)
    {
        if (!isLoading)
        {
            isLoading = true;
            StartCoroutine(SceneSwitchAction(targetScene, playerPos, isFade));
        }
    }

    private IEnumerator SceneSwitchAction(SceneSO targetScene, Vector3 playerPos, bool isFade)
    {
        if (isFade)
        {
            fadeDeleSo.ActiveFadeIn(fadeTime);
        }

        yield return new WaitForSeconds(fadeTime);
        yield return currentScene?.sceneAsset.UnLoadScene();
        //playerTransform.gameObject.SetActive(false);这里不使用setacitve是因为如果这样写，playercontroller中事件注册在事件发生之后而无法禁止玩家输入
        playerTransform.GetComponent<SpriteRenderer>().enabled = false;
        unloadedSceneDeleSo.ActiveDele(targetScene, playerPos, false); //场景unload后动作，角色血条显示与关闭
        
        var loadTask = targetScene.sceneAsset.LoadSceneAsync(LoadSceneMode.Additive);
        loadTask.Completed += OnLoadCompleted;
        
        yield break;

        void OnLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
        {
            currentScene = targetScene;
            playerTransform.position = playerPos;
            //playerTransform.gameObject.SetActive(true);
            playerTransform.GetComponent<SpriteRenderer>().enabled = true;

            if (targetScene.sceneType != SceneType.UIMenu) {afterSceneLoadDeleSo.ActiveDele();} // afterLoad后启动玩家角色移动输入，摄像机边界
            fadeDeleSo.ActiveFadeOut(fadeTime);
            isLoading = false;
        }
    }
}
