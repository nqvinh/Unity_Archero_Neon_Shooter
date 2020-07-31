using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyIdleLogic
{
    Basic,
    IdleByTime,
    NONE
}


public static class EnemyIdleBehaviourFactory
{
    public static EnemyIdleBehaviour GetIdleBehaviourLogic(EnemyIdleLogic idleLogic)
    {
        switch (idleLogic)
        {
            case EnemyIdleLogic.Basic:
                return new EnemyBasicIdle();
            case EnemyIdleLogic.IdleByTime:
                return new EnemyIdleByTime();
            case EnemyIdleLogic.NONE:
                break;
        }
        return null;
    }
}

public class EnemyIdleBehaviour : IBehaviour
{
    protected EnemyController enemyController;
    public virtual void FixedUpdate(GameObject actor)
    {
        
    }

    public virtual void LateUpdate(GameObject actor)
    {
       
    }

    public virtual void OnEnter(GameObject actor,params object[] @params)
    {
        enemyController = actor.GetComponent<EnemyController>();
    }

    public virtual void Update(GameObject actor)
    {
        
    }
}

public class EnemyIdleByTime:EnemyIdleBehaviour
{
    float timeIdle;
    float timeBeginIdlePoint = 0;
    public override void OnEnter(GameObject actor,params object[] @params)
    {
        base.OnEnter(actor, @params);
        if (@params != null && @params.Length > 0)
            timeIdle = float.Parse(@params[0] as string);
        timeBeginIdlePoint = Time.time;

    }

    public override void Update(GameObject actor)
    {
        enemyController.DoLookAtTarget();
        if (Time.time - timeBeginIdlePoint >= timeIdle)
        {
            this.enemyController.GoToState(enemyController.moveState);
        }
    }
}

public class EnemyBasicIdle:EnemyIdleBehaviour
{
    public override void Update(GameObject actor)
    {
        //if mainChar in attack Range then attack
        //else move to mainChar

        float distance = UtilityHandler.Distance(actor.transform.position, enemyController.target.transform.position);
        if (distance < enemyController.enemyConfigRef.refEnemyConfig.attackRange)
        {
            enemyController.GoToState(enemyController.attackState);
        }
        else
        {
            enemyController.GoToState(enemyController.moveState);
        }
    }
}