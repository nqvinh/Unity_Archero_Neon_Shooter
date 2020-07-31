using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBeam : BaseBullet
{
    [SerializeField] BeamFx beamFxPrefabs;

    BeamFx beamIns;

    public override void InitData()
    {
        if (!hasInitData)
        {
            hasInitData = true;
            bulletMovementUpdater = BulletMovementFactory.GetBulletMovementUpdater(movementType);
            bulletHitObjectHandler = BulletHitFactory.GetBulletHitObjectHandler(hitType);
        }

        if (beamIns == null)
        {
            //beamIns = PoolSystem.RequireObject(beamFxPrefabs);
           

            beamIns.onBeamEnd = () =>
            {
                isShooting = false;
            };
        }
        
    }

    //public override void Shot(HeroController hero, MobController target, Transform beginPos)
    //{
       
    //    if (target != this.Target)
    //    {
    //        beamIns.StopBeam();
    //    }

    //    this.Target = target;
    //    this.Hero = hero;
     
    //    //if (bulletFx)
    //    //{
    //    //    bulletFx.Stop();
    //    //    bulletFx.Play();
    //    //}
    //    this.transform.position = beginPos.position;
    //    beamIns.transform.position = beginPos.position;
    //    beamIns.ShotBeam(beginPos, target.center, 2, 0.1f);
    //    beamIns.onHit = () =>
    //    {
    //        OnHit(target.gameObject);
    //    };

    //    isShooting = true;
    //}

    protected override void OnTriggerEnter(Collider other)
    {
      
    }

    protected override void Update()
    {
        if (isShooting)
        {
            beamIns.UpdateBeam();
           
        }
    }
}
