using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyMoveLogic
{
    NavMeshMove,
    RandomMove,
    NONE
}


public static class EnemyMoveBehaviourFactory
{
    public static EnemyMoveBehaviour GetLogic(EnemyMoveLogic moveLogic)
    {
        switch (moveLogic)
        {
            case EnemyMoveLogic.NavMeshMove:
                return new EnemyNavMeshMove();
            case EnemyMoveLogic.RandomMove:
                return new EnemyRandomMove();
            case EnemyMoveLogic.NONE:
                break;
        }
        return null;
    }
}

public class EnemyMoveBehaviour : IBehaviour
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

    public bool CheckTargetInAttackRangeAndAttack()
    {
        if (this.enemyController.DistanceWithTarget() < this.enemyController.enemyConfigRef.refEnemyConfig.attackRange)
        {
            if (this.enemyController.navMeshAgent)
            {
                this.enemyController.navMeshAgent.isStopped = true;
                this.enemyController.navMeshAgent.enabled = false;
            }
            this.enemyController.GoToState(this.enemyController.attackState);
            return true;
        }
        return false;
    }
}



public class EnemyNavMeshMove : EnemyMoveBehaviour
{
 
    public override void OnEnter(GameObject actor, params object[] @params)
    {
        base.OnEnter(actor, @params);
        Debug.Assert(this.enemyController.navMeshAgent, "Enemy Should Have NavMeshAgent");
        this.enemyController.navMeshAgent.enabled = true;
    }
    public override void Update(GameObject actor)
    {
        base.Update(actor);
        this.enemyController.navMeshAgent.SetDestination(this.enemyController.target.transform.position);
        this.CheckTargetInAttackRangeAndAttack();
    }
}


public class EnemyRandomMove : EnemyMoveBehaviour
{
    Vector3 moveTarget;
    public override void OnEnter(GameObject actor, params object[] @params)
    {
        base.OnEnter(actor, @params);
        PickARandomPosAndMove();
    }

    void PickARandomPosAndMove()
    {
        moveTarget = Map.Instance.GetARandomEmptyCellWorldPos();
        enemyController.navMeshAgent.enabled = true;
        enemyController.navMeshAgent.SetDestination(moveTarget);
    }

    public override void Update(GameObject actor)
    {

        Debug.DrawLine(actor.transform.position, moveTarget, Color.red);
        bool canAttack = this.CheckTargetInAttackRangeAndAttack();
        if (!canAttack)
        {
            if (!this.enemyController.navMeshAgent.pathPending)
            {
                if (this.enemyController.navMeshAgent.remainingDistance <= this.enemyController.navMeshAgent.stoppingDistance)
                {
                    PickARandomPosAndMove();
                }
            }
        }
    }
}
