using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private PhysicsCheck _physicsCheck;
    private PlayerController _playerController;

    public void ActiveHurt()
    {
        _animator.SetTrigger("Hurt");
    }

    public void ActiveAttack()
    {
        _animator.SetTrigger("Attack");
    }
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _physicsCheck = GetComponent<PhysicsCheck>();
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        AnimParamSet();
    }

    private void AnimParamSet()
    {
        float criticalSpeed = PlayerController.FixSpeed / 2f;
        _animator.SetBool("isRun", Mathf.Abs(_rigidbody2D.velocity.x) > criticalSpeed);
        _animator.SetBool("isWalk", Mathf.Abs(_rigidbody2D.velocity.x) > 0.01 && Mathf.Abs(_rigidbody2D.velocity.x) <= criticalSpeed);
        _animator.SetFloat("y_Velocity", _rigidbody2D.velocity.y);
        _animator.SetBool("isGround", _physicsCheck.isGround);
        _animator.SetBool("isCrouch", _playerController.isCrouch);
        _animator.SetBool("isDead", _playerController.isDead);
        _animator.SetBool("isAttack", _playerController.isAttack);
        _animator.SetBool("onWall", _playerController.OnWall);
        _animator.SetBool("isSlide", _playerController.isSlide);
    }
}
