using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerConfig:ScriptableObject 
{
    public float playerMoveSpeed = 100;
    public long playerBaseHp =120;
    public bool isAIPlayer = false;
    public float playerAttackSpeed = 1000.0f;
}
