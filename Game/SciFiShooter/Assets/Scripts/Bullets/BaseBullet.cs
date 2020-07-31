using DG.Tweening;
using QuickType;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum BulletType
{
    Missle,
    Spray,
    Beam,
    SubBullet,
    COUNT
}

public class BaseBullet : MonoBehaviour
{
    //Bullet Visual
    [SerializeField] protected ParticleSystem bulletFx;
    [SerializeField] protected ParticleSystem explosionFx;

    //Bullet Data
    public BulletType bulletType;
    public LayerMask hitMask;
    [SerializeField] protected string hitTag;
    [SerializeField] protected float speed;
    public bool Critical { get; set; }
    public int Damage { get; set; }
    
    //public MobController Target { get; set; }
    //public HeroController Hero { get; internal set; }

    //[SerializeField] protected BulletEffect bulletEffect;


    //Bullet Logic
    [SerializeField] protected BulletMovementType movementType;
    [SerializeField] protected BulletHitType hitType;
    protected BulletMovementUpdater bulletMovementUpdater;
    protected IBulletHitObjectHandler bulletHitObjectHandler;

    //Bullet Event

    //public Action<MobController> onHitTarget;



    protected bool hasInitData = false;
    protected bool isShooting = false;
    protected bool hasExplosion = false;

    public virtual void InitData()
    {
        if (!hasInitData)
        {
            hasInitData = true;
            bulletMovementUpdater = BulletMovementFactory.GetBulletMovementUpdater(movementType);
            bulletHitObjectHandler = BulletHitFactory.GetBulletHitObjectHandler(hitType);
        }
        bulletMovementUpdater.baseSpeed = this.speed;
        isShooting = false;
        hasExplosion = false;
    }

    public virtual void InitData(BulletTemplate bulletTemplate,BulletEmitter bulletEmitter)
    {
        if (bulletTemplate.BulletProperties.Type == "Linear")
        {
            bulletMovementUpdater = BulletMovementFactory.GetBulletMovementUpdater(BulletMovementType.MoveForward);
        }
        else if (bulletTemplate.BulletProperties.Type == "Parabol")
        {
            bulletMovementUpdater = BulletMovementFactory.GetBulletMovementUpdater(BulletMovementType.MoveParabol);
        }
        bulletHitObjectHandler = BulletHitFactory.GetBulletHitObjectHandler(hitType);

        bulletMovementUpdater.origin = bulletEmitter.transform;
        bulletMovementUpdater.baseSpeed = bulletTemplate.BulletProperties.Speed;
        bulletMovementUpdater.accelery = bulletTemplate.BulletProperties.Accel;
        bulletMovementUpdater.sprialSpeed = bulletTemplate.BulletProperties.SprialSpeed;
        bulletMovementUpdater.jumpHeight = bulletTemplate.BulletProperties.JumpHeight;
        bulletMovementUpdater.jumpDistance = bulletTemplate.BulletProperties.JumpDistance;
        isShooting = false;
        hasExplosion = false;
    }

    //public virtual void Shot(HeroController hero, MobController target,Transform beginPos)
    //{
    //    this.Target = target;
    //    this.Hero = hero;
    //    this.transform.position = beginPos.position;

    //    if (bulletFx)
    //    {
    //        bulletFx.Stop();
    //        bulletFx.Play();
    //    }
    //    if (bulletMovementUpdater != null)
    //        bulletMovementUpdater.Setup(this, target.transform, beginPos.position);
    //    isShooting = true;
    //}

    public virtual void Shot(Vector3 startPos,Vector3 direction)
    {
        this.transform.position = startPos;
        this.transform.forward = direction;
        if (bulletMovementUpdater != null)
            bulletMovementUpdater.Setup(this,direction, startPos);
        isShooting = true;
    }

    public virtual void Shot(Vector3 startPos,Transform target)
    {
        Vector3 direction = (target.position - startPos).normalized;
        this.transform.position = startPos;
        this.transform.forward = direction;
        if (bulletMovementUpdater != null)
            bulletMovementUpdater.Setup(this, target, startPos);
        isShooting = true;
    }

    


    protected virtual void Update()
    {
        if (isShooting)
        {
            if (hasExplosion)
            {

            }
            else
            {
                if (bulletMovementUpdater != null)
                    bulletMovementUpdater.UpdateMovement(this);
            }
        }
    }

    protected virtual void OnHit(GameObject obj)
    {
     
        if (bulletHitObjectHandler != null)
            bulletHitObjectHandler.ProcessHitObj(this, obj.transform, OnHitTarget);

        if (explosionFx) ParticleEffectManager.Instance.ShowFx(explosionFx.name, this.transform.position, 1f);

        hasExplosion = true;
        isShooting = false;
    }

    void OnHitTarget(GameObject target)
    {
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
    
        if ((!string.IsNullOrEmpty(hitTag) && other.CompareTag(hitTag)) || (hitMask == (hitMask | (1<<other.gameObject.layer))) )
        {
            OnHit(other.gameObject);
        }
    }
}
