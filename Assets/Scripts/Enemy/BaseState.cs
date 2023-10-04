using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//基础状态
public abstract class BaseState
{
    public abstract void OnEnter(Enemy enemy);
    public abstract void LogicUpdate();
    public abstract void PhysicsUpdate();
    public abstract void OnExit();
}
