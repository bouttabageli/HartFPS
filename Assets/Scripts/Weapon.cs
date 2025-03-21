using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
public class Weapon : MonoBehaviour
{
  
    //shooting
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;
    //burst
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;
    //spread
    public float spreadIntensity;
    //bullet
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;

    public GameObject muzzleEffect;
    private Animator animator;

    //ammo
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;
    

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }
    public ShootingMode currentShootingMode;
    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();
        bulletsLeft = magazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        if(bulletsLeft == 0 && isShooting){
            SoundManager.Instance.emptySoundM4.Play();
        }
        if(currentShootingMode == ShootingMode.Auto)
        {
            //holding left mouse down
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if(currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            //clicking left mouse once
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }
        if(Input.GetKey(KeyCode.R)&& bulletsLeft < magazineSize && isReloading == false)
        {
            Reload();
        }
        if(readyToShoot && isShooting && bulletsLeft > 0)
        {
            burstBulletsLeft = bulletsPerBurst;
            
            FireWeapon();
        }

        if(AmmoManager.Instance.ammoDisplay != null){
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft/bulletsPerBurst}/{magazineSize/bulletsPerBurst}";
        }
    }
    private void FireWeapon()
    {
        bulletsLeft--;
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");
        SoundManager.Instance.shootingSoundM4.Play();
        readyToShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
        //instantiate bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        //point the bullet to take the shooting direction
        bullet.transform.forward = shootingDirection;
        //shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        //destroy the bullet after time
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
        if(allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }
        if(currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }
    private void Reload()
    {
        SoundManager.Instance.reloadingSoundM4.Play();
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }
    private void ReloadCompleted()
    {
        bulletsLeft = magazineSize;
        isReloading = false;
    }
    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }
    public Vector3 CalculateDirectionAndSpread()
    {
        //Shooting from the middle of the screen to check where we are pointing
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
        {
            //hitting something
            targetPoint = hit.point;
        } else 
        {
            //shooting at the air
            targetPoint = ray.GetPoint(100);
        }
        Vector3 direction = targetPoint - bulletSpawn.position;
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        return direction + new Vector3(x, y, 0);
    }
    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
