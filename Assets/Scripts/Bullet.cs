using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public int bulletDamage;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Target"))
        {
            print("hit " + collision.gameObject.name + " !");
            CreateBulletImpactEffect(collision);
            Destroy(gameObject);
        }
        if(collision.gameObject.CompareTag("Wall"))
        {
            print("hit a wall");
            CreateBulletImpactEffect(collision);
            Destroy(gameObject);
        }
        if(collision.gameObject.CompareTag("Bottle"))
        {
            print("hit a bottle");
            collision.gameObject.GetComponent<Bottle>().Explode();
        }
        if(collision.gameObject.CompareTag("Enemy"))
        {
            print("hit an enemy");
            if(collision.gameObject.GetComponent<Enemy>().isDead == false)
            {
                collision.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);
            }
            CreateBloodSprayEffect(collision);
            Destroy(gameObject);
        }
    }
    void CreateBulletImpactEffect(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab, 
            contact.point, 
            Quaternion.LookRotation(contact.normal)
        );
        hole.transform.SetParent(collision.gameObject.transform);
    }

    void CreateBloodSprayEffect(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        GameObject bloodSprayPrefab = Instantiate(
            GlobalReferences.Instance.bloodSprayEffect, 
            contact.point, 
            Quaternion.LookRotation(contact.normal)
        );
        bloodSprayPrefab.transform.SetParent(collision.gameObject.transform);
    }
}
