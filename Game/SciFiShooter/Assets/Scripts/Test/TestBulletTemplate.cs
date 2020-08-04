using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBulletTemplate : MonoBehaviour
{
    [SerializeField] PlayerAnimationController player;
    [SerializeField] BulletEmitter emitter;
    [SerializeField] ParticleSystem muzzle;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            muzzle.Play();
            player.Shooting = true;
            emitter.Shot(null);
        }
    }
}
