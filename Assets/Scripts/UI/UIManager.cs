using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

//UI事件监听管理
public class UIManager : MonoBehaviour
{
    //UIManager作为发布者
    public VoidDeleSO gamePauseDeleSo;
    
    //UIManager作为订阅者
    public CharacterDeleSO changeDele; 
    public SceneSwitchDeleSO unloadedSceneDeleSo;
    public VoidDeleSO gameOverDeleSo;
    public VoidDeleSO loadDataDeleSo;
    public VoidDeleSO backToMenuDeleSo;
    public UpdateSliderDeleSO updateSliderDeleSo;
    
    public PlayerStateBar playerStateBar;
    public GameObject gameOverPanel, mobileTouch, pausePanel;
    public Button restartButton, settingButton;
    public Slider masterVolumeSlider, bgmVolumeSlider, fxVolumeSlider;
    
    private void Awake()
    {
        //预编译指令
        #if UNITY_STANDALONE
        mobileTouch.SetActive(false);
        #endif
        
        settingButton.onClick.AddListener(ToggleSettingPanel);
    }
    
    private void OnEnable()
    {
        changeDele.OnEventDele += HealthHandle;
        changeDele.OnEventDele += PowerHandle;
        unloadedSceneDeleSo.OnEventDele += UnloadedSceneAction;
        loadDataDeleSo.OnEventDele += OnLoadDataAction;
        gameOverDeleSo.OnEventDele += OnGameOverAction;
        backToMenuDeleSo.OnEventDele += OnLoadDataAction;
        updateSliderDeleSo.OnEventDele += UpdateSliderAction;
    }
    
    private void OnDisable()
    {
        changeDele.OnEventDele -= HealthHandle;
        changeDele.OnEventDele -= PowerHandle;
        unloadedSceneDeleSo.OnEventDele -= UnloadedSceneAction;
        loadDataDeleSo.OnEventDele -= OnLoadDataAction;
        gameOverDeleSo.OnEventDele -= OnGameOverAction;
        backToMenuDeleSo.OnEventDele -= OnLoadDataAction;
        updateSliderDeleSo.OnEventDele -= UpdateSliderAction;
    }
    
    private void HealthHandle(Character character)
    {
        var temp = character.currentHealth / character.maxHealth;
        playerStateBar.healthImageChange(temp);
    }

    private void PowerHandle(Character character)
    {
        var temp = character.currentPower / character.maxPower;
        playerStateBar.powerImageChange(temp);
    }
    
    //场景切换控制人物血条UI
    private void UnloadedSceneAction(SceneSO arg0, Vector3 arg1, bool arg2) =>
        playerStateBar.gameObject.SetActive(arg0.sceneType != SceneType.UIMenu);
    
    private void OnLoadDataAction() => gameOverPanel.SetActive(false);
    
    private void OnGameOverAction()
    {
        gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
    }
    
    private void ToggleSettingPanel()
    {
        if (pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
            gamePauseDeleSo.ActiveDele();
        }
    }

    private void UpdateSliderAction(float temp, int index)
    {
        switch (index)
        {
            case 1:
                masterVolumeSlider.value = (temp + 80) / 100;
                break;
            case 2:
                bgmVolumeSlider.value = (temp + 80) / 100;
                break;
            case 3:
                fxVolumeSlider.value = (temp + 80) / 100;
                break;
        }
    }
}
