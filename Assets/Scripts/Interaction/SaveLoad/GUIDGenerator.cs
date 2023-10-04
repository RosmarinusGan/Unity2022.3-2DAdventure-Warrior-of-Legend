using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIDGenerator : MonoBehaviour
{
    public GUIDType type;
    public string GUID;
    
    //当脚本加载或在编辑器面板改变某数值时，立马调用无论游戏是否在运行
    private void OnValidate()
    {
        if (type == GUIDType.ReadWrite)
        {
            if (GUID == string.Empty) //这里不能用null判断,似乎因为默认为string.Empty而不是null
            {
                GUID = System.Guid.NewGuid().ToString();
            }
        }
        else
        {
            GUID = null;
        }
    }
}
