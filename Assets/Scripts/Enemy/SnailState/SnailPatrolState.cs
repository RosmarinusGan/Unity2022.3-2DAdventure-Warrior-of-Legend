using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailPatrolState : BaseState
{
    private Snail snail;
    public override void OnEnter(Enemy enemy)
    {
        snail = enemy as Snail; // snail可空
        if(snail == null) return;
        
        snail.currentSpeed = snail.normalSpeed;
    }

    public override void LogicUpdate()
    {
        if(snail == null) return;
        
        //巡逻时碰墙或悬崖等待一段时间
        if (!snail.physicsCheck.isGround || (snail.physicsCheck.isLeftWall && !snail.isFaceRight) || (snail.physicsCheck.isRightWall && snail.isFaceRight))
        {
            snail.isWait = true;
        }
        snail.TimeCounter();
        
        //发现player切换到skill状态, 要放在 巡逻碰墙或悬崖 逻辑后，否则无法打断等待状态
        if (snail.SightCheck())
        {
            snail.StateSwitch(snail.snailSkillState);
        }
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        if(snail == null) return;
        
        snail.isWait = false;
    }
}
