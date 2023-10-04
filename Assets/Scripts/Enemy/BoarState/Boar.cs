using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//关于动画状态机的操作应写在Enemy子类当中，应为每一个敌人的动画状态机都是不同的
public class Boar : Enemy
{
    internal BoarPatrolState boarPatrolState = new BoarPatrolState();
    internal BoarChaseState boarChaseState = new BoarChaseState();
    
    //受伤动画播放(添加到事件)
    public void ActiveHurt()
    {
        animator.SetTrigger("hurt");
    }
    
    //死亡动画播放(添加到事件)
    public void ActiveDead()
    {
        isDead = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
        animator.SetTrigger("dead");
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        StateSwitch(boarPatrolState);
    }

    protected override void Update()
    {
        base.Update();
        //动画状态设置
        animator.SetBool("isWalk", Mathf.Abs(rb2D.velocity.x) > 0.1 && Mathf.Abs(rb2D.velocity.x) <= normalSpeed + 0.1);
        animator.SetBool("isRun", Mathf.Abs(rb2D.velocity.x) > normalSpeed + 0.1);
    }
}
