using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snail : Enemy
{
    internal SnailPatrolState snailPatrolState = new SnailPatrolState();
    internal SnailSkillState snailSkillState = new SnailSkillState();
    
    public bool isHide { get; set; }
    
    //受伤动画播放(添加到事件)
    public void ActiveHurt()
    {
        animator.SetTrigger("hurt");
        StateSwitch(snailSkillState);
    }
    
    //死亡动画播放(添加到事件)
    public void ActiveDead()
    {
        isDead = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
        animator.SetTrigger("dead");
    }

    protected override void Move()
    {
        //蜗牛独有的移动约束，即位于snail premove状态时速度几乎为0，即不移动
        currentSpeed = animator.GetCurrentAnimatorStateInfo(0).IsName("snailPreMove") ? 0.1f : normalSpeed; // 这里不能为0，因为需要处于walk状态
        
        //处于hide和恢复状态没有速度
        if (isHide) return;
        currentSpeed = animator.GetCurrentAnimatorStateInfo(0).IsName("snailRecover") ? 0f : normalSpeed; //要位于isHide判断后，因为处于hide状态下不应该执行该句

        base.Move();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StateSwitch(snailPatrolState);
    }
    
    protected override void Update()
    {
        base.Update();
        //动画状态设置
        animator.SetBool("isWalk", Mathf.Abs(rb2D.velocity.x) > 0 && Mathf.Abs(rb2D.velocity.x) <= normalSpeed + 0.1);
        animator.SetBool("isHide", isHide);
    }
}
