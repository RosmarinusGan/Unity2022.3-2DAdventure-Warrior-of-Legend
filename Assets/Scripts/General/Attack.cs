using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*攻击伤害,攻击给被施加方的力度,攻击给被施加方的受力方向的修正*/
public class Attack : MonoBehaviour
{
    public int damage;

    public float attackForce;

    public Vector2 forceDirOffset = Vector2.up;
    
    [Header("Only Bee")]
    public float attackRange;
    public float attackRate;

    private void OnTriggerStay2D(Collider2D other)
    {
        other.GetComponent<Character>()?.TakeDamage(this);
    }
    
}
