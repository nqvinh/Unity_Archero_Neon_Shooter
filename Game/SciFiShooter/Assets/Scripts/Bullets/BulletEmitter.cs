using QuickType;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BulletEmitter : MonoBehaviour
{

    [SerializeField] private AssetReference baseBullet;

    [SerializeField] private AssetReference _addressableTextAsset = null;

    public PoolObject<BaseBullet> bulletPool;
    BulletTemplate bulletTemplate;
    IBulletTypeShotHandler bulletTypeShotHandler = null;
    EmitterProperties emitterProperties;
    bool isShoting = false;
    int curremtWave = 0;

    public bool CanShot
    {
        get
        {
            return (bulletTemplate != null && !isShoting && bulletPool.IsReady) ;
        }
    }
    public System.Action<BulletEmitter> onShotDone;

    private void Start()
    {
        bulletPool = new PoolObject<BaseBullet>();
        InitBulletPool();
        _addressableTextAsset.LoadAssetAsync<TextAsset>().Completed += BulletQuad_Completed;
      
    }

    async void InitBulletPool()
    {
        GameObject bulletPrefab = await baseBullet.LoadAssetAsync<GameObject>().Task;
        BaseBullet baseBulletComp = bulletPrefab.GetComponent<BaseBullet>();
        bulletPool.CreatePool(baseBulletComp, 50);
        baseBullet.ReleaseAsset();
    }

    private void BulletQuad_Completed(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<TextAsset> obj)
    {
        bulletTemplate = BulletTemplate.FromJson(obj.Result.text);
        string currentBulletType = bulletTemplate.BulletType.CurrentType;
        switch(currentBulletType)
        {
            case "BulletSpray":
                bulletTypeShotHandler = new BulletSprayShot();
                break;
            case "BulletDirection":
                bulletTypeShotHandler = new BulletDirectionShot();
                break;
            case "BulletTemplate":
                bulletTypeShotHandler = new BulletTemplateShot();
                break;
        }
        //Debug.LogError(bulletTemplate.BulletTempalte);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
          
        }

        if (isShoting && bulletTemplate.EmitterProperties.SpinSpeed > 0)
        {
            Vector3 currentRotate = this.transform.localEulerAngles;
            currentRotate.y += bulletTemplate.EmitterProperties.SpinSpeed;
            this.transform.localEulerAngles = currentRotate;
        }
    }

    public void Shot(Transform target)
    {

        if (!CanShot) return;
        isShoting = true;
        curremtWave = 0;
        StartCoroutine(ShotByWave(target));
    }

    IEnumerator ShotByWave(Transform target)
    {
        for (int i=0;i<bulletTemplate.EmitterProperties.Wave;++i)
        {
            ShotBullet(target);
            yield return new WaitForSeconds(bulletTemplate.EmitterProperties.Rate);
        }
        isShoting = false;
        onShotDone?.Invoke(this);
    }

    private void ShotBullet(Transform target=null)
    {
     
        string currentBulletType = bulletTemplate.BulletType.CurrentType;
        QuickType.Bullet bullet = Array.Find(bulletTemplate.BulletType.Bullet, b => b.Type.CompareTo(currentBulletType) == 0);

        if (target) this.transform.LookAt(target);
        bulletTypeShotHandler.Shot(this, bullet, bulletTemplate, target);        
      
        //Debug.Break();
    }
}


public interface IBulletTypeShotHandler
{
    void Shot(BulletEmitter emitter,QuickType.Bullet bulletData,BulletTemplate bulletTemplate,Transform target);
}

public class BulletSprayShot : IBulletTypeShotHandler
{
    public void Shot(BulletEmitter emitter,QuickType.Bullet bulletData,BulletTemplate bulletTemplate,Transform target)
    {
        int minAngle = bulletData.Properties.MinAngle;
        int maxAngle = bulletData.Properties.MaxAngle;
        int numSpray = bulletData.Properties.NumSpray;

        float angleEach = Math.Abs(maxAngle - minAngle) / (numSpray - 1);

        float angle = minAngle;

        for (int i=0;i<numSpray;++i)
        {
            Vector3 dir = Quaternion.Euler(new Vector3(0, angle, 0)) * emitter.transform.forward;
            BaseBullet bullet = emitter.bulletPool.GetNextObject().GetComponent<BaseBullet>();
            bullet.transform.position = emitter.transform.position;
            bullet.InitData(bulletTemplate,emitter);
            if (target)
            {
                bullet.Shot(emitter.transform.position, target);
            }
            else
            {
                bullet.Shot(emitter.transform.position, -dir);
            }
                
            angle += angleEach;
        }
    }
}

public class BulletDirectionShot : IBulletTypeShotHandler
{
    public void Shot(BulletEmitter emitter, QuickType.Bullet bulletData, BulletTemplate bulletTemplate,Transform target)
    {
        int numDir = bulletData.Properties.Angle == 0? 1: 360 / bulletData.Properties.Angle;
        float angle = 0;

        for (int i = 0; i < numDir; ++i)
        {
            Vector3 dir = Quaternion.Euler(new Vector3(0, angle, 0)) * emitter.transform.forward;
            BaseBullet bullet = emitter.bulletPool.GetNextObject().GetComponent<BaseBullet>();
            bullet.transform.position = emitter.transform.position;
            bullet.InitData(bulletTemplate, emitter);
            if (target)
                bullet.Shot(emitter.transform.position, target);
            else
                bullet.Shot(emitter.transform.position, -dir);
            angle += bulletData.Properties.Angle;
        }
    }
}


public class BulletTemplateShot : IBulletTypeShotHandler
{
    const float unitStep = 0.5f;
    const float quadSize = 21;

    public void Shot(BulletEmitter emitter, QuickType.Bullet bulletData, BulletTemplate bulletTemplate,Transform target)
    {
        Vector3 origin = emitter.transform.position;

        float minX = -Mathf.FloorToInt(quadSize * 0.5f) * unitStep;
        float maxY = Mathf.FloorToInt(quadSize * 0.5f) * unitStep;

        int templateLeght = bulletData.Properties.Template.Length;
        //float baseAngle = 45;
        for (int i = 0; i < templateLeght; ++i)
        {
            float x = origin.x + minX + bulletData.Properties.Template[i].Col * unitStep;
            float z = origin.z + maxY - bulletData.Properties.Template[i].Row * unitStep;

            Vector3 pos = new Vector3(x, emitter.transform.position.y, z);

            Vector3 norDir = pos - emitter.transform.position;
            norDir = Quaternion.Euler(emitter.transform.localEulerAngles) * norDir;
            pos = norDir + emitter.transform.position;

            //pos = this.transform.rotation * pos;

            BaseBullet bullet = emitter.bulletPool.GetNextObject().GetComponent<BaseBullet>();
            bullet.transform.position = pos;
            bullet.transform.forward = -emitter.transform.forward;
            Vector3 dir = bullet.transform.forward;
            dir = Quaternion.Euler(0, -bulletData.Properties.Template[i].Dir , 0) * dir;

            bullet.InitData(bulletTemplate,emitter);

            if (target)
                bullet.Shot(pos, target);
            else
                bullet.Shot(pos, dir);
        }
    }
}