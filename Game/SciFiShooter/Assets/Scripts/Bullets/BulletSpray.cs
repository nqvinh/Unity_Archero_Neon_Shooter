using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpray : BaseBullet
{
    public override void InitData()
    {
        base.InitData();
    }

    //public override void Shot(HeroController hero, MobController target, Transform beginPos)
    //{
    //    this.Target = target;
    //    this.Hero = hero;
     
    //    if (bulletFx)
    //    {
    //        bulletFx.Stop();
    //        bulletFx.Play();
    //    }
       
    //    isShooting = true;
    //}

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void Update()
    {
        
    }
}
