using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState
{
    private Boar boar;
    public override void OnEnter(Enemy enemy)
    {
        boar = enemy as Boar; //boar可空
        if(boar == null) return;
        
        boar.currentSpeed = boar.normalSpeed;
    }

    public override void LogicUpdate()
    {
        if(boar == null) return;
        
        //巡逻时碰墙或悬崖等待一段时间
        if (!boar.physicsCheck.isGround || (boar.physicsCheck.isLeftWall && !boar.isFaceRight) || (boar.physicsCheck.isRightWall && boar.isFaceRight))
        {
            boar.isWait = true;
        }
        boar.TimeCounter();
        
        //发现player切换到chase状态, 要放在 巡逻碰墙或悬崖 逻辑后，否则无法打断等待状态
        if (boar.SightCheck())
        {
            boar.StateSwitch(boar.boarChaseState);
        }
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        if(boar == null) return;
        
        boar.isWait = false; //如果在等待状态，则要打断改状态，否则敌人就不会动了
    }
}
