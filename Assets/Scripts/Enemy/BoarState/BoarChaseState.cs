using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState
{
    private Boar boar;
    public override void OnEnter(Enemy enemy)
    {
        boar = enemy as Boar; //boar可空
        if(boar == null) return;
        
        boar.currentSpeed = boar.chaseSpeed;
    }

    public override void LogicUpdate()
    {
        if(boar == null) return;
        
        //巡逻时碰墙或悬崖立即转身
        if (!boar.physicsCheck.isGround || (boar.physicsCheck.isLeftWall && !boar.isFaceRight) || (boar.physicsCheck.isRightWall && boar.isFaceRight))
        {
            boar.isFaceRight = !boar.isFaceRight;
            boar.transform.localScale = new Vector3(boar.isFaceRight ? -1 : 1, boar.transform.localScale.y, boar.transform.localScale.z); // 这里必须立即执行反转，否则在遇到悬崖时会反转多次
        }
        boar.TimeCounter();
        
        //丢失玩家状态转换
        if (boar.chaseCountTime <= 0)
        {
            boar.StateSwitch(boar.boarPatrolState);
        }
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        
    }
}
