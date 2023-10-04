using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeePatrolState : BaseState
{
    private Bee bee;
    private Vector2 currentTarget;
    
    public override void OnEnter(Enemy enemy)
    {
        bee = enemy as Bee; //bee可空
        if(bee == null) return;

        bee.currentSpeed = bee.normalSpeed;
        currentTarget = GetTargetPos();
    }

    public override void LogicUpdate()
    {
        if (bee.SightCheck())
        {
            bee.StateSwitch(bee.beeChaseState);
        }

        if (Mathf.Abs(currentTarget.x - bee.transform.position.x) < 0.1f &&
            Mathf.Abs(currentTarget.y - bee.transform.position.y) < 0.1f)
        {
            bee.isWait = true;
            bee.rb2D.velocity = Vector2.zero;
            currentTarget = GetTargetPos();
        }
        bee.TimeCounter();
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        
    }
    
    //获取给定移动范围内的随机点
    private Vector2 GetTargetPos()
    {
        var temp = (Vector2)bee.originPos + new Vector2(Random.Range(-bee.patrolDistance, bee.patrolDistance),
            Random.Range(-bee.patrolDistance, bee.patrolDistance));
        bee.moveDir = (temp - (Vector2)bee.transform.position).normalized;
        return temp;
    }
}
