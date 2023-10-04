using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailSkillState : BaseState
{
    private Snail snail;
    public override void OnEnter(Enemy enemy)
    {
        snail = enemy as Snail; // snail可空
        if(snail == null) return;
        
        snail.isHide = true;
        snail.rb2D.velocity = Vector2.zero;
        snail.animator.SetTrigger("hide"); //状态机中必须要有这个trigger，否则会不断进入hide状态，而不是维持在hide idle，因为any state。
        snail.GetComponent<Character>().isInvincible = true;
    }

    public override void LogicUpdate()
    {
        if(snail == null) return;
        
        snail.TimeCounter();
        //丢失玩家状态转换
        if (snail.chaseCountTime <= 0)
        {
            snail.StateSwitch(snail.snailPatrolState);
        }
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        if(snail == null) return;
        
        snail.isHide = false;
        snail.GetComponent<Character>().isInvincible = false;
    }
}
