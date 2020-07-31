using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyConfigRef
{
    public string enemyConfigId;
    public EnemyConfig refEnemyConfig;
}

public class EnemyController : MonoBehaviour
{
    [SerializeField] FSMController fsmController;
    [SerializeField] public Rigidbody rigidbody;

    public EnemyAnimationController enemyAnimController;
    public EnemyRenderState enemyRenderState;

    public EnemySpawningState spawningState;
    public EnemyIdleState idleState;
    public EnemyAttackState attackState;
    public EnemyMoveState moveState;
    public EnemyDeadState deadState;


    public BoxCollider hitBox;

    public PlayerController target;
    public EnemyConfigRef enemyConfigRef;
    public NavMeshAgent navMeshAgent;

    private bool _isFilledByData = false;
    private float rotateSpeed = 3.14f;

    private long currentHP, maxHP;

    public System.Action<EnemyController> onDead;

    public bool Alive { get { return currentHP > 0; } }

    private void Awake()
    {
        
    }

    private void Start()
    {
        InitData();
    }

    public void InitData()
    {
        if (_isFilledByData) return;
        enemyConfigRef.refEnemyConfig = GameConfig.Instance.enemyConfigs.Find(e => e.configName == enemyConfigRef.enemyConfigId);
        _isFilledByData = true;

        currentHP = maxHP = enemyConfigRef.refEnemyConfig.maxHP;

        spawningState = new EnemySpawningState(gameObject);
        idleState = new EnemyIdleState(gameObject);
        attackState = new EnemyAttackState(gameObject);
        moveState = new EnemyMoveState(gameObject);
        deadState = new EnemyDeadState(gameObject);

        //IDle ENUM
        //Load from data
        string[] idleParam = enemyConfigRef.refEnemyConfig.idleParams.Length > 0? enemyConfigRef.refEnemyConfig.idleParams.Split(','): null;
        string[] moveParam = enemyConfigRef.refEnemyConfig.moveParams.Length > 0 ? enemyConfigRef.refEnemyConfig.moveParams.Split(',') : null;
        string[] attackParam = enemyConfigRef.refEnemyConfig.attackParams.Length > 0 ? enemyConfigRef.refEnemyConfig.attackParams.Split(',') : null;
        string[] deadParam = enemyConfigRef.refEnemyConfig.deadParams.Length > 0 ? enemyConfigRef.refEnemyConfig.deadParams.Split(',') : null;
        idleState.SetBehaviour(EnemyIdleBehaviourFactory.GetIdleBehaviourLogic(enemyConfigRef.refEnemyConfig.idleLogic),idleParam);
        moveState.SetBehaviour(EnemyMoveBehaviourFactory.GetLogic(enemyConfigRef.refEnemyConfig.moveLogic),moveParam);
        attackState.SetBehaviour(EnemyAttackBehaviourFactory.GetLogic(enemyConfigRef.refEnemyConfig.attackLogic),attackParam);
        deadState.SetBehaviour(EnemyDeadBehaviourFactory.GetLogic(enemyConfigRef.refEnemyConfig.deadLogic), deadParam);

        fsmController.actor = gameObject;
        fsmController.AddState(spawningState);
        fsmController.AddState(idleState);
        fsmController.AddState(moveState);
        fsmController.AddState(attackState);
        fsmController.AddState(deadState);

        fsmController.GoToState(spawningState);
    }

    public void OnHit(long dame)
    {
        if (currentHP <= 0)
            return;
        Debug.LogError(string.Format("On Get Hit Dame {0}", dame));
        currentHP -= dame;
        if (currentHP <= 0)
        {
            onDead?.Invoke(this);
        
            fsmController.GoToState(deadState);
        }
            
    }


    public void SetTarget(PlayerController playerController)
    {
        this.target = playerController;
    }
    

    public void GoToState(BaseState state)
    {
        //Enemy Die but change to active state must return
        if (currentHP <= 0 && state != deadState)
            return;
        fsmController.GoToState(state);
    }

    private void Update()
    {
        fsmController.UpdateState();
    }

    public  void DoLookAtTarget()
    {
        Vector3 moveVector = this.target.transform.position - this.transform.position;
        var currentRotationSpeed = rotateSpeed * Mathf.Max(0, (1 - 0.3f) / 0.7f);

        // The step size is equal to speed times frame time.
        float singleStep = currentRotationSpeed * Time.deltaTime ;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, moveVector.normalized, singleStep, 0.0f);

        Quaternion nextRotation = Quaternion.LookRotation(newDirection);

        rigidbody.MoveRotation(nextRotation);

    }

    public float DistanceWithTarget()
    {
        float distance = UtilityHandler.Distance(this.transform.position, target.transform.position);
        return distance;
    }

}