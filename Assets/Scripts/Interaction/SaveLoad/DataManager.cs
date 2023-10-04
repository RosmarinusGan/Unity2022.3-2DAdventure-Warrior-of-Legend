using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

[DefaultExecutionOrder(-100)]
public sealed class DataManager : MonoBehaviour
{
    //单例
    #region Singleton
    /*原生C#写法
    private DataManager(){}
    private static readonly Lazy<DataManager> lazy = new Lazy<DataManager>(() => new DataManager());
    public static DataManager Instance => lazy.Value;
    */
    /*MonoBehaviour下写法*/
    /* 这种写法此时不可行，因为有事件变量，此情况似乎启动时无法订阅事件
    private static DataManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }else Destroy(this);

        data = new Data();
    }
    
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "Data Manager";
                    instance = obj.AddComponent<DataManager>();
                    instance.SaveDataDeleSo = AssetDatabase.LoadAssetAtPath<VoidDeleSO>("Assets/Data SO/EventDeleSO/Save Data Dele SO.asset");
                    DontDestroyOnLoad(instance);
                }
            }
            return instance;
        }
    }
    */
    public static DataManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }else Destroy(this);

        data = new Data();
        jsonFolderPath = Application.persistentDataPath + "/Game Data/";
        dataPath = jsonFolderPath + "data.sav";
    }
    #endregion
    
    //DataManager作为订阅者
    public VoidDeleSO SaveDataDeleSo;
    public VoidDeleSO LoadDataDeleSo;
    public VoidDeleSO afterSceneLoadDeleSo;
    
    //DataManager作为发布者
    public VoidDeleSO NewGameDeleSo; 
    
    private List<ISaveable> registeredISaveable = new ();
    private Data data; //临时存档
    private string jsonFolderPath, dataPath; //持久化存档

    public void Register(ISaveable obj)
    {
        if (!registeredISaveable.Contains(obj))
        {
            registeredISaveable.Add(obj);
        }
    }

    public void UnRegister(ISaveable obj)
    {
        if (registeredISaveable.Contains(obj))
        {
            registeredISaveable.Remove(obj);
        }
    }

    public void UpdateInteractableState(string GUID, bool state)
    {
        data.interactDict[GUID] = state;
    }
    
    //新游戏删除存档
    public void DeleteData()
    {
        if (File.Exists(dataPath))
        {
            File.Delete(dataPath);
        }

        data = new Data();
    }
    
    private void OnEnable()
    {
        SaveDataDeleSo.OnEventDele += SaveAction;
        LoadDataDeleSo.OnEventDele += LoadAction;
        afterSceneLoadDeleSo.OnEventDele += OnLoadInteractableAction;
    }
    
    private void OnDisable()
    {
        SaveDataDeleSo.OnEventDele -= SaveAction;
        LoadDataDeleSo.OnEventDele -= LoadAction;
        afterSceneLoadDeleSo.OnEventDele -= OnLoadInteractableAction;
    }
    
    //读取一遍当前所有数据并存硬盘
    private void SaveAction()
    {
        foreach (var obj in registeredISaveable.Where(obj => obj.GetGUID() != null)) //当obj的GUID为空的时候，不执行其senddata
        {
            obj.SendData(data);
        }
        
        if (!File.Exists(dataPath))
        {
            Directory.CreateDirectory(jsonFolderPath);
        }
        File.WriteAllText(dataPath, JsonConvert.SerializeObject(data));
    }
    
    //本意上是加载所有物体，但常常只加载场景与玩家角色
    private void LoadAction()
    {
        if(!File.Exists(dataPath)) NewGameDeleSo.ActiveDele();
        ReadDataFromJson();
        foreach (var obj in registeredISaveable.Where(obj => obj.GetGUID() != null))
        {
            obj.LoadData(data);
        }
    }
    
    //每次场景加载完后，加载交互物体的状态
    private void OnLoadInteractableAction()
    {
        foreach (var obj in registeredISaveable.Where(obj => obj.GetGUID() != null))
        {
            if (obj is IInteractable)
            {
                obj.LoadData(data);
            }
        }
    }
    
    //从硬盘读json数据
    private void ReadDataFromJson()
    {
        if (File.Exists(dataPath))
        {
            data = JsonConvert.DeserializeObject<Data>(File.ReadAllText(dataPath));
        }
    }
}
