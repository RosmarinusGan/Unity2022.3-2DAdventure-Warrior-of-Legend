using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimation : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //注意这里有空档期，因为state machine状态切换的条件判断(evaluating this state)在状态退出(OnStateExit)之前,
        //所以在 前一个状态退出后 到 下一次攻击键被按下(在当前状态执行中某个时间点)前 isAttack都为false
        animator.GetComponent<PlayerController>().isAttack = true;
        //这里保证在攻击状态执行中isAttack始终为true
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // 注意这个函数在Has Exit Time没有被勾选时不会执行
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerController>().isAttack = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
