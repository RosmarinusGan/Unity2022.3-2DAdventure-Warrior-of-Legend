using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GUIDGenerator))]
public abstract class SaveBehaviour : MonoBehaviour, ISaveable
{
    protected virtual void OnEnable() => ((ISaveable) this).RegisterOnManager();
    protected virtual void OnDisable() => ((ISaveable) this).UnRegisterOnManager();

    public string GetGUID() => GetComponent<GUIDGenerator>().type == GUIDType.ReadWrite ? GetComponent<GUIDGenerator>().GUID : null;
    public abstract void SendData(Data data);
    public abstract void LoadData(Data data);
}
