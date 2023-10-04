using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SavePoint : SaveBehaviour, IInteractable
{
    public Sprite lightSprite, darkSprite;
    public SpriteRenderer spriteRenderer;
    public Light2D lightSource;
    
    //SavePoint作为发布者
    public VoidDeleSO SaveDataDele;
    public bool isDone { get; set; }
    
    public void DoInteraction()
    {
        if (!isDone)
        {
            spriteRenderer.sprite = lightSprite;
            isDone = true;
            lightSource.gameObject.SetActive(true);
        }
        else
        {
            lightSource.GetComponent<Animator>().Play("lightAnim");
        }
        
        //数据保存硬盘
        SaveDataDele.ActiveDele();
    }

    public override void SendData(Data data)
    {
        if (data.interactDict.ContainsKey(GetGUID()))
        {
            data.interactDict[GetGUID()] = isDone;
        }
        else
        {
            data.interactDict.Add(GetGUID(), isDone);
        }
    }

    public override void LoadData(Data data)
    {
        if (data.interactDict.ContainsKey(GetGUID()))
        {
            isDone = data.interactDict[GetGUID()];
            if (isDone)
            {
                spriteRenderer.sprite = lightSprite;
                lightSource.gameObject.SetActive(true);
            }
        }
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        //保持宝箱的状态(通过序列化isDone的值)
        spriteRenderer.sprite = isDone ? lightSprite : darkSprite;
        lightSource.gameObject.SetActive(isDone);
    }
}
