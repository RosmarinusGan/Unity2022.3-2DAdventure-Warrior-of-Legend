using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
/*unity中常用的属性，get得到一个复制，set使用引用*/
/*start,awake函数仅在gameobject是active下调用，且只调用一次无论之后gameobject的active状态，OnEnable在每次被active下调用*/
/*玩家固有性质*/
public partial class PlayerController : MonoBehaviour
{
    public static readonly float FixSpeed = 6f;
    public float jumpForce;
    public float slideDistance;
    public float slideSpeed;
    public float slidePower;
    public PhysicsMaterial2D wall;
    public PhysicsMaterial2D normal;
    
    //PlayerController作为订阅者
    public SceneSwitchDeleSO sceneSwitchDeleSo;
    public VoidDeleSO afterSceneLoadDeleSo;
    public VoidDeleSO loadDataDeleSo;
    public VoidDeleSO backToMenuDeleSo;
    
    private Vector2 _inputDirection;
    private float _moveSpeed = FixSpeed;
    private PlayerInputControl _playerInputControl;
    private Rigidbody2D _rigidbody2D;
    private PhysicsCheck _physicsCheck;
    private CapsuleCollider2D _capsuleCollider2D;
    private PlayerAnimation _playerAnimation;
    private Vector2 _originOffset;
    private Vector2 _originSize;
    private bool isOnWallJump;
    
    //受伤
    public bool isHurt { get; set; }
    //死亡
    public bool isDead { get; set; }
    //攻击
    public bool isAttack { get; set; }
    //下蹲
    public bool isCrouch => (Mathf.Abs(_rigidbody2D.velocity.x) < 3.5f || GetComponent<Character>().currentPower - slidePower < 0)
                                 && _physicsCheck.isGround 
                                 && _inputDirection.y < -0.1f;
    //滑铲
    public bool isSlide { get; set; }
    //爬墙
    public bool OnWall { get; set; }
    
    //受伤击退(添加到事件)
    public void HurtBeat(Attack attack)
    {
        isHurt = true;
        _rigidbody2D.velocity = Vector2.zero;
        var dir = (new Vector2(transform.position.x, transform.position.y) - new Vector2(attack.transform.position.x, attack.transform.position.y)).normalized +
                  attack.forceDirOffset;
        _rigidbody2D.AddForce(dir.normalized * attack.attackForce, ForceMode2D.Impulse);
    }
    
    //玩家死亡(添加到事件)
    public void PlayerDead()
    {
        //玩家死亡状态处理
        //_playerInputControl.Gameplay.Disable(); 不采用此方法，因为PlayerMove()还是会被调用导致玩家速度归零
        isDead = true;
        //注意如果动画状态机Any State进入Dead动画没有勾选Has Exit Time，那么之前的Hurt动画的OnStateExit函数并没有被调用，那么此时isHurt仍然为true。
        
        // 死亡状态退出
        // isDead = false;
    }
    
    private void Awake()
    {
        _playerInputControl = new PlayerInputControl();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _physicsCheck = GetComponent<PhysicsCheck>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _playerAnimation = GetComponent<PlayerAnimation>();
        _originOffset = _capsuleCollider2D.offset;
        _originSize = _capsuleCollider2D.size;
        
        //跳跃
        _playerInputControl.Gameplay.Jump.started += context => { Jump(); };
        
        //攻击
        _playerInputControl.Gameplay.Attack.started += context => { Attack(); };
        
        //走路
        float walkSpeed = FixSpeed / 2.5f;
        float runSpeed = FixSpeed;
        _playerInputControl.Gameplay.Walk.performed += context =>
        {
            _moveSpeed = walkSpeed;
        };
        _playerInputControl.Gameplay.Walk.canceled += context =>
        {
            _moveSpeed = runSpeed;
        };
    }

    private void OnEnable()
    {
        _playerInputControl.Enable(); 
        sceneSwitchDeleSo.OnEventDele += SceneSwitchAction;
        afterSceneLoadDeleSo.OnEventDele += AfterLoadAction;
        loadDataDeleSo.OnEventDele += OnLoadDataAction;
        backToMenuDeleSo.OnEventDele += OnLoadDataAction;
    }
    
    private void OnDisable()
    {
        _playerInputControl.Disable();
        sceneSwitchDeleSo.OnEventDele -= SceneSwitchAction;
        afterSceneLoadDeleSo.OnEventDele -= AfterLoadAction;
        loadDataDeleSo.OnEventDele -= OnLoadDataAction;
        backToMenuDeleSo.OnEventDele -= OnLoadDataAction;
    }
    
    //场景切换控制用户输入
    private void SceneSwitchAction(SceneSO arg0, Vector3 arg1, bool arg2) => _playerInputControl.Gameplay.Disable();
    private void AfterLoadAction() => _playerInputControl.Gameplay.Enable();
    
    private void OnLoadDataAction()
    {
        isDead = false;
    }
    
    private void FixedUpdate()
    {
        _inputDirection = _playerInputControl.Gameplay.Move.ReadValue<Vector2>();
        if (!isHurt && !isDead && !isAttack && !isOnWallJump && !OnWall && !isSlide)
        {
            PlayerMove();
        }
    }

    private void Update()
    {
        OnWallCheck();
        OnSlideCheck();
        CheckState();
    }
    
    //输入控制玩家速度进行移动
    private void PlayerMove()
    {
        //移动与翻转
        _rigidbody2D.velocity = !isCrouch 
            ? new Vector2(_inputDirection.x * _moveSpeed * (1 / Time.deltaTime) * Time.deltaTime,
                _rigidbody2D.velocity.y) 
            : new Vector2(0, 0);
        if(_inputDirection.x != 0){
            //GetComponent<SpriteRenderer>().flipX = _inputDirection.x < 0; 无法使用这个方法，因为子物体attackArea不翻转
            transform.localScale = new Vector3(_inputDirection.x < 0 ? -1 : 1, transform.localScale.y, transform.localScale.z);
        }
        
        //下蹲修改碰撞体
        if (isCrouch)
        {
            _capsuleCollider2D.offset = new Vector2(_capsuleCollider2D.offset.x, 0.85f);
            _capsuleCollider2D.size = new Vector2(_capsuleCollider2D.size.x, 1.7f);
        }
        else
        {
            _capsuleCollider2D.offset = _originOffset;
            _capsuleCollider2D.size = _originSize;
        }
    }
    
    //滑铲判断
    private float targetX;
    private void OnSlideCheck()
    {
        //开始滑铲条件判断
        if (Mathf.Abs(_rigidbody2D.velocity.x) >= 3.5f && _physicsCheck.isGround && _inputDirection.y < -0.1f) //注意这里使用3.5作为阈值，因为用户input的Vector2的值取决于wasd四个键(四个方向的单位向量)，当sd同时按下，_inputDirection等于(0，-1)(1，0)两个向量的和
        {
            if (!isSlide && GetComponent<Character>().currentPower - slidePower >= 0)
            {
                isSlide = true;
                GetComponent<Character>().isInvincible = true;
                GetComponent<Character>().CostPower(slidePower);
                targetX = transform.position.x + transform.localScale.x * slideDistance;
            }
        }
        
        //滑铲过程
        if (isSlide)
        {
            _rigidbody2D.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed,
                transform.position.y));
            if (Mathf.Abs(targetX - transform.position.x) < 0.1f 
                || !_physicsCheck.isGround 
                || (_physicsCheck.isLeftWall && transform.localScale.x < 0) 
                || (_physicsCheck.isRightWall && transform.localScale.x > 0))
            {
                isSlide = false;
                _rigidbody2D.velocity = Vector2.zero;
                GetComponent<Character>().isInvincible = false;
            }
        }
    }
    
    //滑墙或蹬墙判断
    private void OnWallCheck()
    {
        if (!OnWall && !isOnWallJump) //这里必须要不在Wall Jump状态下，否则蹬墙跳会黏墙，因为下面在OnWall状态下会重新设置速度
        {
            OnWall = (_physicsCheck.isLeftWall && _inputDirection.x < 0f) ||
                     (_physicsCheck.isRightWall && _inputDirection.x > 0f);
        }

        if (OnWall)
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _rigidbody2D.velocity.y / 2);
            if ((!_physicsCheck.isLeftWall && !_physicsCheck.isRightWall) || _inputDirection.y < -0.1f)
            {
                OnWall = false;
            }
        }
    }
    
    private void CheckState()
    {
        //玩家死亡后避免被鞭尸
        gameObject.layer = isDead ? LayerMask.NameToLayer("Enemy") : LayerMask.NameToLayer("Player");
        
        //跳跃修改物理材质
        _capsuleCollider2D.sharedMaterial = _physicsCheck.isGround ? normal : wall;
        
        //玩家蹬墙跳状态
        if (isOnWallJump && _rigidbody2D.velocity.y < 2f)
        {
            isOnWallJump = false;
        }
        
        //蹬墙跳允许调整转向，以免蹬墙跳后短时间再次滑墙但没有执行playermove而导致人物没有转向
        if(_inputDirection.x != 0 && isOnWallJump){
            transform.localScale = new Vector3(_inputDirection.x < 0 ? -1 : 1, transform.localScale.y, transform.localScale.z);
        }
    }
    
    private void Jump()
    {
        if(isDead) return;
        
        // 按下跳跃直接打断滑铲
        if (isSlide)
        {
            isSlide = false;
            _rigidbody2D.velocity = Vector2.zero;
            GetComponent<Character>().isInvincible = false; 
        }

        //一次跳跃
        if (_physicsCheck.isGround && !OnWall)
        {
            _rigidbody2D.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            
            //播放跳跃音效
            GetComponent<AudioDefinition>().ActiveAudioDele();
        }
        
        //蹬墙跳
        if (OnWall)
        {
            OnWall = false; //这里必须要设置离开OnWall状态否则会黏墙
            Vector2 baseForce = new Vector2(-transform.localScale.x, 1) * jumpForce * 0.5f; //基础力
            Vector2 inputForce = new Vector2(_inputDirection.x * 8f, 3f); //输入力
            _rigidbody2D.AddForce(baseForce + inputForce, ForceMode2D.Impulse);
            isOnWallJump = true;
        }
    }

    private void Attack()
    {
        if (!isDead && !OnWall)
        {
            isAttack = true;
            _playerAnimation.ActiveAttack();
        }
    }
}
