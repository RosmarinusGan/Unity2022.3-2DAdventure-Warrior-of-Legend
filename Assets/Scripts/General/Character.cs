using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/*与外界交互的属性*/
/*血量、无敌时间*/
public class Character : SaveBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public float invincibleTime;
    public bool isInvincible;
    [Header("Only Player")] 
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;
    
    public UnityEvent<Attack> hurtEvent;
    public UnityEvent deadEvent;
    public UnityEvent<Character> OnHPChange; //当血量或者能量发生变化,Character作为发布者

    public VoidDeleSO newGameDele; // Character作为订阅者
    
    public void TakeDamage(Attack attack)
    {
        if(!isInvincible)
        {
            currentHealth = currentHealth - attack.damage < 0 ? 0 : currentHealth - attack.damage;
            
            //受伤
            if (currentHealth > 0)
            {
                hurtEvent?.Invoke(attack);
            }

            //死亡
            if (currentHealth <= 0)
            {
                deadEvent?.Invoke();
            }
            
            OnHPChange?.Invoke(this);
            
            //无敌时间
            if (!isInvincible) //这里需要判断因为可能受伤或死亡事件调用改变了无敌状态
            {
                isInvincible = true;
                StartCoroutine(TimeDelay());
            }
        }
    }

    public void CostPower(float cost)
    {
        currentPower = currentPower - cost >= 0 ? currentPower - cost : 0f;
        OnHPChange?.Invoke(this);
    }
    
    public override void SendData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetGUID()))
        {
            data.characterPosDict[GetGUID()] = transform.position;
            data.hpDict[GetGUID() + "health"] = currentHealth;
            data.hpDict[GetGUID() + "power"] = currentPower;
        }
        else
        {
            data.characterPosDict.Add(GetGUID(), transform.position);
            data.hpDict.Add(GetGUID() + "health", currentHealth);
            data.hpDict.Add(GetGUID() + "power", currentPower);
        }
    }

    public override void LoadData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetGUID()))
        {
            transform.position = data.characterPosDict[GetGUID()];
            currentHealth = data.hpDict[GetGUID() + "health"];
            currentPower = data.hpDict[GetGUID() + "power"];
            OnHPChange?.Invoke(this);
        }
    }
    
    //碰到水体死亡
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            currentHealth = 0f;
            OnHPChange?.Invoke(this);
            deadEvent?.Invoke();
        }
    }
    
    private void Initialize()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        isInvincible = false;
        OnHPChange?.Invoke(this);
    }

    private void Start() => currentHealth = maxHealth;

    protected override void OnEnable()
    {
        base.OnEnable();
        newGameDele.OnEventDele += Initialize;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        newGameDele.OnEventDele -= Initialize;
    }

    private void Update()
    {
        //能量回复
        if (currentPower < maxPower)
        {
            currentPower += powerRecoverSpeed * Time.deltaTime;
            OnHPChange?.Invoke(this);
        }
    }

    private IEnumerator TimeDelay()
    {
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }
}
