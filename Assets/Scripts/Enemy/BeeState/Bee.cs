using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : Enemy
{
    public float patrolDistance;
    
    internal Vector3 originPos;
    internal Vector2 moveDir;
    internal BeePatrolState beePatrolState = new ();
    internal BeeChaseState beeChaseState = new ();

    public bool isChase { get; set; }
    public bool isAttack { get; set; }
    
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
    
    public override bool SightCheck()
    {
        var temp = Physics2D.OverlapCircle(transform.position, checkDistance, playerLayer);
        beeChaseState.playerTargetTransfrom = temp?.gameObject.transform;
        return temp;
    }

    protected override void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, checkDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(originPos, patrolDistance);
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        originPos = transform.position;
        StateSwitch(beePatrolState);
    }

    protected override void Move()
    {
        if(isAttack) return;
        
        //移动速度
        rb2D.velocity = moveDir * (currentSpeed * (1 / Time.deltaTime) * Time.deltaTime);
        
        //翻转
        if (rb2D.velocity.x != 0)
        {
            transform.localScale =
                new Vector3(rb2D.velocity.x < 0 ? 1 : -1, transform.localScale.y, transform.localScale.z);
        }
    }

    protected override void Update()
    {
        base.Update();
        animator.SetBool("isChase", isChase);
    }
}
