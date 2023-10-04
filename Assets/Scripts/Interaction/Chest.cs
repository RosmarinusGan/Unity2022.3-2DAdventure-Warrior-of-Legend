using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : SaveBehaviour, IInteractable
{
    public Sprite openSprite, closeSprite;
    private SpriteRenderer spriteRenderer;
    
    public bool isDone { get; set; }
    
    public void DoInteraction()
    {
        if (!isDone)
        {
            spriteRenderer.sprite = openSprite;
            isDone = true;
            gameObject.tag = "Untagged";
            DataManager.Instance.UpdateInteractableState(GetGUID(), isDone);
            GetComponent<AudioDefinition>()?.ActiveAudioDele();
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        //保持宝箱的状态(通过序列化isDone的值)
        spriteRenderer.sprite = isDone ? openSprite : closeSprite;
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
                spriteRenderer.sprite = openSprite;
                gameObject.tag = "Untagged";
            }
        }
    }
}
