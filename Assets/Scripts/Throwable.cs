using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class Throwable : MonoBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] float damageRadius = 20f;
    [SerializeField] float explosionForce = 1200f;
    [SerializeField] int damage = 0;

    float countdown;
    bool hasExploded = false;
    public bool hasBeenThrown = false;

    public enum ThrowableType
    {
        None,
        Grenade,
        Smoke
    } 
    public ThrowableType throwableType;

    private void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        if(hasBeenThrown)
        {
            countdown -= Time.deltaTime;
            if(countdown <= 0f && !hasExploded)
            {
                Explode();
                hasExploded = true;
            }
        }
    }

    private void Explode()
    {
        GetThrowableEffect();
        Destroy(gameObject);
    }

    private void GetThrowableEffect()
    {
        switch(throwableType)
        {
            case ThrowableType.Grenade:
                GrenadeEffect();
                break;
            case ThrowableType.Smoke:
                SmokeGrenadeEffect();
                break;
        }
    }

    private void GrenadeEffect()
    {
        GameObject explosionEffect = GlobalReferences.Instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);

        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.grenadeSound);

        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach(Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }

            if(objectInRange.gameObject.GetComponent<Enemy>())
            {
                objectInRange.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            }
        }
    }

    private void SmokeGrenadeEffect()
    {
        //Visual
        GameObject smokeEffect = GlobalReferences.Instance.smokeGrenadeEffect;
        Instantiate(smokeEffect, transform.position, transform.rotation);
        //Play Sound
        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.grenadeSound);
        //Physical Effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach(Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if(rb != null)
            {
                //apply blindness to enemy
            }
        }
    }


}
