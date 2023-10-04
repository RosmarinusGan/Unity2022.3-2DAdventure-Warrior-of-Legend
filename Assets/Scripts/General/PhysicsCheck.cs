using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//注意玩家的攻击碰撞体的物体图层应设置为非player的其他图层，否则武器会被识别为player，可能引发某些问题
public class PhysicsCheck : MonoBehaviour
{
    public Vector2 bottomOffset;
    public float checkRadius;
    public LayerMask groundMask;
    public bool isGround, isLeftWall, isRightWall;
    
    private Vector2 collSize, collOffset;
    private void Awake()
    {
        collSize = GetComponent<CapsuleCollider2D>().size;
        collOffset = GetComponent<CapsuleCollider2D>().offset;
    }

    private void Update()
    {
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y),
            checkRadius,
            groundMask);
        isLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + collOffset + new Vector2(-collSize.x / 2, 0),
            checkRadius,
            groundMask);
        isRightWall = Physics2D.OverlapCircle((Vector2)transform.position + collOffset + new Vector2(collSize.x / 2, 0),
            checkRadius,
            groundMask);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y),
            checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + collOffset + new Vector2(-collSize.x / 2, 0),
            checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + collOffset + new Vector2(collSize.x / 2, 0),
            checkRadius);
    }
}
