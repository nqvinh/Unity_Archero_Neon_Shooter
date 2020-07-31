using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyDeadLogic
{
    Basic,
    NONE
}


public static class EnemyDeadBehaviourFactory
{
    public static EnemyDeadBehaviour GetLogic(EnemyDeadLogic idleLogic)
    {
        switch (idleLogic)
        {
            case EnemyDeadLogic.Basic:
                return new EnemyDeadSink();
                break;
        }
        return null;
    }
}

public class EnemyDeadBehaviour : IBehaviour
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


        if (this.enemyController.navMeshAgent)
        {
            this.enemyController.navMeshAgent.isStopped = true;
            this.enemyController.navMeshAgent.enabled = false;
        }
        this.enemyController.enemyRenderState.SetActiveHighLightFx(false, Color.red);
        this.enemyController.enemyAnimController.Dead = true;

      
    }

    public virtual void Update(GameObject actor)
    {
        
    }
}


public class EnemyDeadSink : EnemyDeadBehaviour
{
    public override void OnEnter(GameObject actor, params object[] @params)
    {
        base.OnEnter(actor, @params);

        float deadAnimTime = float.Parse(@params[0] as string);

        DOVirtual.DelayedCall(deadAnimTime, () =>
        {
            float skinkTime = float.Parse(@params[1] as string);
            this.enemyController.rigidbody.isKinematic = true;
            this.enemyController.hitBox.gameObject.SetActive(false);
            Vector3 curPos = this.enemyController.transform.position;
            curPos.y -= 4;
            this.enemyController.transform.DOMove(curPos, skinkTime).onComplete = () =>
            {
                this.enemyController.gameObject.SetActive(false);
            };
        });
    }
}