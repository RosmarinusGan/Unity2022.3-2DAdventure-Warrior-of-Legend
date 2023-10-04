using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public GameObject newGameButton;
    
    //退出游戏
    public void ExitGame()
    {
        Application.Quit();
    }
    
    private void OnEnable()
    {
        //使屏幕默认focus在new game button上
        EventSystem.current.SetSelectedGameObject(newGameButton);
    }
}
