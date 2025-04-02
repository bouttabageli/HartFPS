using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;

    public int weaponDamage;
    [Header("Shooting")]
    //shooting
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;
    [Header("Burst")]
    //burst
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;
    [Header("Spread")]
    //spread
    public float spreadIntensity;
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;
    [Header("Bullet")]
    //bullet
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;

    public GameObject muzzleEffect;
    private Animator animator;
    [Header("Ammo")]
    //ammo
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;
    [Header("Spawn settings")]
    //spawn settings
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
    public Vector3 spawnScale;
    
    bool isADS;

    public enum WeaponModel
    {
        M4A1,
        Skorpion
    }

    public WeaponModel thisweaponModel;

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
        spreadIntensity = hipSpreadIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        if(isActiveWeapon)
        {
            foreach(Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("WeaponRender");
            }
            if(Input.GetMouseButtonDown(1))
            {
                EnterADS();
            }
             if(Input.GetMouseButtonUp(1))
            {
                ExitADS();
            }
            if(bulletsLeft == 0 && isShooting)
            {
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
            if(Input.GetKey(KeyCode.R)&& bulletsLeft < magazineSize && isReloading == false && WeaponManager.Instance.CheckAmmoLeftFor(thisweaponModel) > 0)
            {
                Reload();
            }
            if(readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
            
                FireWeapon();
            }

        }
        else 
        {
            foreach(Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }
    private void EnterADS()
    {
        animator.SetTrigger("enterADS");
        isADS = true;
        HUDManager.Instance.crosshair.SetActive(false);
        spreadIntensity = adsSpreadIntensity;
    }
    private void ExitADS()
    {
        animator.SetTrigger("exitADS");
        isADS = false;
        HUDManager.Instance.crosshair.SetActive(true);
        spreadIntensity = hipSpreadIntensity;
    }
    private void FireWeapon()
    {
        bulletsLeft--;
        muzzleEffect.GetComponent<ParticleSystem>().Play();

        if(isADS)
        {
            animator.SetTrigger("RECOIL_ADS");
        }
        else 
        {
            animator.SetTrigger("RECOIL");
        }
        
        SoundManager.Instance.PlayShootingSound(thisweaponModel);
        readyToShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
        //instantiate bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        Bullet bul = bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;
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
        SoundManager.Instance.PlayReloadSound(thisweaponModel);
        animator.SetTrigger("RELOAD");
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }
    private void ReloadCompleted()
    {
        if(WeaponManager.Instance.CheckAmmoLeftFor(thisweaponModel) > magazineSize)
        {
            bulletsLeft = magazineSize;
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisweaponModel);
        }
        else
        {
            bulletsLeft = WeaponManager.Instance.CheckAmmoLeftFor(thisweaponModel);
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisweaponModel);
        }
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
        Vector3 spreadOffset = Camera.main.transform.right * x + Camera.main.transform.up * y;

        return (direction + spreadOffset).normalized;
        
    }
    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

}
