using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameConfig : SingletonScriptObject<GameConfig>
{
    [SerializeField]
    public List<EnemyConfig> enemyConfigs = new List<EnemyConfig>();
}
