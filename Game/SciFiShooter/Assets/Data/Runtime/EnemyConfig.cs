using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyConfig : BaseConfig,ICloneable<EnemyConfig>
{
    public long maxHP;
    public long dame;
    public float attackRange;
    public float attackSpeed;
    public float moveSpeed;
    public bool aimToTarget;

    public EnemyIdleLogic idleLogic;
    public string idleParams;
    public EnemyMoveLogic moveLogic;
    public string moveParams;
    public EnemyAttackLogic attackLogic;
    public string attackParams;
    public EnemyDeadLogic deadLogic;
    public string deadParams;

    public EnemyConfig(string name):base(name)
    {

    }

    public EnemyConfig Clone()
    {
        EnemyConfig enemyConfig = new EnemyConfig("New Clone Enemy");
        enemyConfig.maxHP = this.maxHP;
        enemyConfig.dame = this.dame;
        enemyConfig.attackRange = this.attackRange;
        enemyConfig.attackSpeed = this.attackSpeed;
        enemyConfig.moveSpeed = this.moveSpeed;
        enemyConfig.aimToTarget = this.aimToTarget;
        enemyConfig.idleLogic = this.idleLogic;
        enemyConfig.moveLogic = this.moveLogic;
        enemyConfig.attackLogic = this.attackLogic;
        return enemyConfig;
    }
}
