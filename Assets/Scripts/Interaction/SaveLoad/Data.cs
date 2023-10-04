using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class Data
{
    public Dictionary<string, SerializeVector3> characterPosDict = new (); // 这里不使用Vector3，因为其无法被序列化保存
    public Dictionary<string, float> hpDict = new ();
    public Dictionary<string, bool> interactDict = new ();
    [JsonProperty] //需要用这个特性装饰，否则序列化时不保存private变量
    private string sceneData;

    public void SaveScene(SceneSO scene) => sceneData = JsonUtility.ToJson(scene);

    public SceneSO LoadScene()
    {
        var temp = ScriptableObject.CreateInstance<SceneSO>();
        JsonUtility.FromJsonOverwrite(sceneData, temp);
        return temp;
    }
    
    public class SerializeVector3
    {
        [JsonProperty]
        private float x, y, z;

        public SerializeVector3(Vector3 data)
        {
            this.x = data.x;
            this.y = data.y;
            this.z = data.z;
        }
        
        public static implicit operator Vector3(SerializeVector3 data) => new (data.x, data.y, data.z);
        public static implicit operator SerializeVector3(Vector3 data) => new (data);
    }
}
