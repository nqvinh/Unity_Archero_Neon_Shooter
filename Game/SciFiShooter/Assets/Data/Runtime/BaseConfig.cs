using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BaseConfig : ScriptableObject
{
    public string id;
    public string configName = "";
    public BaseConfig(string configName)
    {
        this.configName = configName;
        id = System.Guid.NewGuid().ToString();
    }
}
