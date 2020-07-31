using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyAttackLogic
{
    MeleeAttack,
    AttackWithBulletTemplate,
    NONE
}


public static class EnemyAttackBehaviourFactory
{
    public static EnemyAttackBehaviour GetLogic(EnemyAttackLogic attackLogic)
    {
        switch (attackLogic)
        {
            case EnemyAttackLogic.MeleeAttack:
                return new EnemyAttackMelee();
            case EnemyAttackLogic.AttackWithBulletTemplate:
                return new EnemyAttackByBulletTemplate();
        }
        return null;
    }
}

public class EnemyAttackBehaviour : IBehaviour
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



public class EnemyAttackMelee : EnemyAttackBehaviour
{
    public override void FixedUpdate(GameObject actor)
    {
        //if (obj.hasDeath || obj == null || obj.gameObject == null)
        //    return;

        float dist = UtilityHandler.Distance(actor.transform.position, enemyController.target.transform.position);
        
        //if (dist <= obj.GetAttackRanage())
        //{
        //    //obj.enemyDatabinding.Move = false;
        //    //obj.navAgent.enabled = false;
        //    //obj.GotoState(obj.attackState);
        //}
        //else
        //{
        //    //obj.enemyDatabinding.Move = true;
        //    //obj.navAgent.enabled = true;
        //    //obj.navAgent.SetDestination(obj.target.position);
        //}
    }
}

public class EnemyAttackByBulletTemplate : EnemyAttackBehaviour
{
    BulletEmitter bulletEmitter;
    bool hasShoot = false;
    bool lookAtTarget = false;
    public override void FixedUpdate(GameObject actor)
    {
        base.FixedUpdate(actor);
    }

    public override void LateUpdate(GameObject actor)
    {
        base.LateUpdate(actor);
    }

    public override void OnEnter(GameObject actor,params object[] @params)
    {
        base.OnEnter(actor,@params);
        if (bulletEmitter == null) bulletEmitter = actor.GetComponentInChildren<BulletEmitter>();
        Debug.Assert(bulletEmitter != null);
        hasShoot = false;
        bulletEmitter.onShotDone = OnAttackDone;
       
    }

    void OnAttackDone(BulletEmitter bulletEmitter)
    {
        enemyController.GoToState(enemyController.idleState);
        enemyController.enemyAnimController.Attacking = false;

    }

    public override void Update(GameObject actor)
    {
        base.Update(actor);
        enemyController.DoLookAtTarget();
        if (!hasShoot && bulletEmitter.CanShot)
        {
            hasShoot = true;
            bulletEmitter.Shot(enemyController.target.HitBox.transform);
        }
    }
}

