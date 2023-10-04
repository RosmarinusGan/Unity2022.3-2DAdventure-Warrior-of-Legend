using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PhysicsCheck))]
//void ActiveHurt; abstract void ActiveDead;
//击退转向写在Character中，ActiveHurt，ActiveDead也许应该抽象成接口
public class Enemy : MonoBehaviour
{
    public float normalSpeed, chaseSpeed;
    
    [HideInInspector]public bool isFaceRight;
    [HideInInspector]public Rigidbody2D rb2D;
    [HideInInspector]public Animator animator;
    [HideInInspector]public PhysicsCheck physicsCheck;
    [HideInInspector]public float currentSpeed;

    [Header("Sight Check")] 
    public Vector2 originOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask playerLayer;
    
    [Header("Patrol Waiting Time")] public float waitTime, countTime; //等待计数器
    [Header("Chase Time Slot")] public float chaseTime, chaseCountTime; //玩家视野(追击)计数器
    
    protected BaseState currentState; // 有限状态机

    public bool isHurt { get; set; }
    public bool isDead { get; set; }
    public bool isWait { get; set; }
    
    //受伤击退与转向(添加到事件)
    public void HurtBeat(Attack attack)
    {
        isHurt = true;
        rb2D.velocity = Vector2.zero;
        //转向
        if (transform.position.x - attack.transform.position.x != 0)
        {
            isFaceRight = transform.position.x - attack.transform.position.x < 0;
        }

        //击退
        rb2D.velocity = Vector2.zero;
        var dir = (new Vector2(transform.position.x, transform.position.y) - new Vector2(attack.transform.position.x, attack.transform.position.y)).normalized +
                  attack.forceDirOffset;
        rb2D.AddForce(dir.normalized * attack.attackForce, ForceMode2D.Impulse);
    }
    
    //死亡清除(添加到animation事件)
    public void EnemyDestroy() => Destroy(this);
    
    //状态切换
    public void StateSwitch(BaseState baseState)
    {
        currentState?.OnExit();
        currentState = baseState;
        currentState?.OnEnter(this);
    }
    
    //计时器
    public void TimeCounter()
    {
        //巡逻时撞墙或悬崖 等待时间
        if (isWait)
        {
            currentSpeed = 0f;
            countTime -= Time.deltaTime;
            if (countTime <= 0)
            {
                currentSpeed = normalSpeed;
                isFaceRight = !isFaceRight;
                countTime = waitTime;
                isWait = false;
                Move();
            }
        }
        else
        {
            countTime = waitTime;
        }
        
        //追逐时丢失视野 追逐时间
        if (!SightCheck())
        {
            chaseCountTime = chaseCountTime - Time.deltaTime <= 0 ? 0 : chaseCountTime - Time.deltaTime;
        }
        else
        {
            chaseCountTime = chaseTime;
        }
    }
    
    //敌人的视野检测玩家
    public virtual bool SightCheck()
    {
        return Physics2D.BoxCast((Vector2)transform.position + originOffset,
            checkSize,
            0,
            new Vector2(isFaceRight ? 1 : -1, 0),
            checkDistance,
            playerLayer);
    }
    
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + originOffset + new Vector2(isFaceRight ? checkDistance : -checkDistance, 0),
            checkSize.x);
    }

    protected virtual void Move()
    {
        //移动速度
        rb2D.velocity =
            new Vector2((isFaceRight ? 1 : -1) * currentSpeed * (1 / Time.deltaTime) * Time.deltaTime,
                rb2D.velocity.y);
        //翻转
        //GetComponent<SpriteRenderer>().flipX = isFaceRight;
        transform.localScale = new Vector3(isFaceRight ? -1 : 1, transform.localScale.y, transform.localScale.z);
    }
    
    protected virtual void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
    }

    protected virtual void OnEnable()
    {
        isFaceRight = false;
        currentState = null;
    }

    protected virtual void FixedUpdate()
    {
        //处于受击，死亡，等待状态不移动
        if (!isHurt && !isDead && !isWait)
        {
            Move();
        }
        currentState?.PhysicsUpdate();
    }

    protected virtual void Update()
    {
        currentState?.LogicUpdate();
    }
}
