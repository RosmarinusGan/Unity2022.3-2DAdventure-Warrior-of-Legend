using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    string GetGUID();
    void RegisterOnManager() => DataManager.Instance.Register(this);
    void UnRegisterOnManager() => DataManager.Instance.UnRegister(this);
    void SendData(Data data);
    void LoadData(Data data);
}
