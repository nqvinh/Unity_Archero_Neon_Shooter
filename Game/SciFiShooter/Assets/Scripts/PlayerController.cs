using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    FootStepRenderer stepQuad;

    [SerializeField]
    BaseBullet baseBullePrefab;

    [SerializeField] ParticleSystem bulletMuzzleFx;

    [SerializeField]
    Transform leftLegPoint;

    [SerializeField]
    Transform rightLegPoint;

    [SerializeField]
    LayerMask groundMask;


    [SerializeField]
    Joystick joystick;

    [SerializeField]
    float moveSpeed;

    private Rigidbody rigidbody;

    [SerializeField]
    private PlayerAnimationController animator;

    [SerializeField]
    private float stepDistance = 0f;

    [SerializeField]
    Transform bulletEmitter;

    Vector3 offsetStep = new Vector3(0, 0.02f, 0);


    PoolObject<FootStepRenderer> poolObject = new PoolObject<FootStepRenderer>();
    PoolObject<BaseBullet> poolBulletObject = new PoolObject<BaseBullet>();

    float moveTimePoint = 0;
    int stepCount = 0;
    float attackSpeed = 0.3f;
    float lastShootTimePoint = 0;

    EnemyController focusingEnemy = null;

    public Rigidbody Rigidbody { get { if (rigidbody == null) rigidbody = GetComponent<Rigidbody>(); return rigidbody; } }

    public BoxCollider HitBox;

    private void Start()
    {
        poolObject.CreatePool(stepQuad, 10);
        poolBulletObject.CreatePool(baseBullePrefab, 100);
    }

    private void Update()
    {
      
    }

    void FixedUpdate()
    {
        float h = 0, v = 0;
        if (joystick != null)
        {
            h = joystick.Horizontal;
            v = joystick.Vertical;
        }
        else
        {
          
        }

        bool running = h != 0f || v != 0f;

        if (running)
        {
            Move(h, v);

            if (focusingEnemy) focusingEnemy.enemyRenderState.SetActiveHighLightFx(false,Color.red);
            focusingEnemy = null;
            animator.Shooting = false;

            if (moveTimePoint == 0 || Time.time - moveTimePoint > stepDistance)
            {
                OnStep();
                moveTimePoint = Time.time;
            }
        }
        else
        {
            moveTimePoint = 0;
            stepCount = 0;
            Shoot();
        }
        animator.Running = running;
        
    }

    public void Shoot()
    {
        if (Time.time - lastShootTimePoint < attackSpeed)
            return;
        if (focusingEnemy && !focusingEnemy.Alive) focusingEnemy = null;
        var nearestEnemy = focusingEnemy?focusingEnemy : Map.Instance.GetNearestEnemy(this.transform.position);
        if (nearestEnemy != null)
        {
            focusingEnemy = nearestEnemy;
            bulletMuzzleFx.Play();
            this.transform.LookAt(nearestEnemy.hitBox.transform);
            bulletEmitter.transform.LookAt(nearestEnemy.hitBox.transform);
            BaseBullet bullet = poolBulletObject.GetNextObject();
            bullet.Damage = 10;
            bullet.InitData();
            Vector3 dir = nearestEnemy.hitBox.transform.position - bulletEmitter.position;
            bullet.Shot(this.bulletEmitter.position, dir.normalized);
            animator.Shooting = true;

            focusingEnemy.enemyRenderState.SetActiveHighLightFx(true,Color.red);
        }
        else
        {
            animator.Shooting = false;
            animator.Running = false;
        }
        lastShootTimePoint = Time.time;
    }


    public void OnHit(long dame)
    {
        Debug.LogError(string.Format("On Get Hit Dame {0}", dame));
    }

    void Move(float h, float v)
    {      


        Vector3 movement = new Vector3(h, 0, v);
        
        // Normalise the movement vector and make it proportional to the speed per second.
        movement = movement.normalized * moveSpeed * Time.deltaTime;
        movement.y = 0;
        Vector3 curPos = transform.position;
        curPos.y = 0;
        Rigidbody.MovePosition(curPos + movement);
        Turning();
    }

    void Turning()
    {
        if (joystick != null)
        {
            Vector3 moveVector = (Vector3.right * joystick.Horizontal + Vector3.forward * joystick.Vertical);
            Rigidbody.MoveRotation(Quaternion.LookRotation(moveVector));
        }
    }

    void OnStep()
    {
        RaycastHit raycastHit;
        if (stepCount % 2 == 0)
        {
            Physics.Raycast(leftLegPoint.position, -Vector3.up, out raycastHit, float.MaxValue, groundMask);
        }
        else
        {
            Physics.Raycast(rightLegPoint.position, -Vector3.up, out raycastHit, float.MaxValue, groundMask);
        }

        if (raycastHit.collider)
        {
            FootStepRenderer step = poolObject.GetNextObject();
            step.SetVisible();
            step.transform.position = raycastHit.point + offsetStep;
            step.transform.localEulerAngles = new Vector3(90, this.transform.localEulerAngles.y, 0);
        }

        stepCount++;
    }

    public void OnLeftFoot()
    {
        //OnStep();
    }

    public void OnRightFoot()
    {
        //OnStep();
    }
}
