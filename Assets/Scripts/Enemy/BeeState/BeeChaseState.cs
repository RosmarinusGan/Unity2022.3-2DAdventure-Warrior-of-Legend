using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeChaseState : BaseState
{
    internal Transform playerTargetTransfrom;
    private Bee bee;
    private Vector2 currentTarget;
    private float attackCounter;
    private Attack attack;
    private bool hasAttack;
    
    public override void OnEnter(Enemy enemy)
    {
        bee = enemy as Bee; //bee可空
        if(bee == null) return;

        bee.currentSpeed = bee.chaseSpeed;
        bee.isChase = true;
        attack = bee.GetComponent<Attack>();
    }

    public override void LogicUpdate()
    {
        if(bee == null) return;
        
        GetTargetPos();
        
        //进入攻击距离
        bee.isAttack = (currentTarget - (Vector2)bee.transform.position).sqrMagnitude <= attack.attackRange && !bee.isHurt; //没有受击
        if (bee.isAttack)
        {
            bee.rb2D.velocity = Vector2.zero;
            if (bee.SightCheck() && !hasAttack) //玩家可能离开视野，离开视野不应该播放攻击动画
            {
                bee.animator.SetTrigger("attack");
                hasAttack = true; 
            }
        }
        attackTimeCounter();
        
        //丢失玩家状态转换
        if (bee.chaseCountTime <= 0)
        {
            bee.StateSwitch(bee.beePatrolState);
        }
        bee.TimeCounter();
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        if(bee == null) return;

        bee.isChase = false;
        bee.isAttack = false;
    }
    
    //获取玩家位置与蜜蜂移动方向
    private void GetTargetPos()
    {
        if(playerTargetTransfrom == null) return;
        
        currentTarget = new Vector2(playerTargetTransfrom.position.x,
            playerTargetTransfrom.position.y + playerTargetTransfrom.GetComponent<CapsuleCollider2D>().size.y / 2);
        bee.moveDir = (currentTarget - (Vector2)bee.transform.position).normalized;
    }

    private void attackTimeCounter()
    {
        if (hasAttack)
        {
            attackCounter -= Time.deltaTime;
            if (attackCounter <= 0)
            {
                attackCounter = attack.attackRate;
                hasAttack = false;
            }
        }
    }
}
