using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; set; }
    public List<GameObject> weaponSlots;
    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;

    [Header("Throwables General")]
    public float forceMultiplier = 0;
    public float forceMultiplierLimit = 2f;
    public float throwForce = 10f;
    public GameObject throwableSpawn;
    
    [Header("Lethals")]
    public int maxLethals = 2;
    public int lethalsCount = 0;
    public GameObject grenadePrefab;
    public Throwable.ThrowableType equippedLethalType;

    [Header("Tacticals")]
    public int maxTacticals = 2;
    public int tacticalCount = 0;
    public Throwable.ThrowableType equippedTacticalType;
    public GameObject smokeGrenadePrefab;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else 
        {
            Instance = this;
        }
    }

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];
        equippedLethalType = Throwable.ThrowableType.None;
        equippedTacticalType = Throwable.ThrowableType.None;
    }

    private void Update()
    {
        foreach(GameObject weaponSlot in weaponSlots)
        {
            if(weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
            }
            else
            {
                weaponSlot.SetActive(false);
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchActiveSlot(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchActiveSlot(1);
        }
        if(Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.T))
        {
            forceMultiplier += Time.deltaTime;

            if(forceMultiplier > forceMultiplierLimit)
            {
                forceMultiplier = forceMultiplierLimit;
            }
        }
        if(Input.GetKeyUp(KeyCode.G))
        {
            if(lethalsCount > 0)
            {
                ThrowLethal();
            }
            forceMultiplier = 0;
        }

        if(Input.GetKeyUp(KeyCode.T))
        {
            if(tacticalCount > 0)
            {
                ThrowTactical();
            }
            forceMultiplier = 0;
        }
    }
    private void ThrowLethal()
    {
        GameObject lethalPrefab = GetThrowablePrefab(equippedLethalType);
        GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();
        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);
        throwable.GetComponent<Throwable>().hasBeenThrown = true;
        lethalsCount -= 1;
        if(lethalsCount <= 0)
        {
            equippedLethalType = Throwable.ThrowableType.None;
        }
        HUDManager.Instance.UpdateThrowables();
    }

    private void ThrowTactical()
    {
        GameObject tacticalPrefab = GetThrowablePrefab(equippedTacticalType);
        GameObject throwable = Instantiate(tacticalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();
        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);
        throwable.GetComponent<Throwable>().hasBeenThrown = true;
        tacticalCount -= 1;
        if(tacticalCount <= 0)
        {
            equippedTacticalType = Throwable.ThrowableType.None;
        }
        HUDManager.Instance.UpdateThrowables();
    }

    private GameObject GetThrowablePrefab(Throwable.ThrowableType equippedThrowableType)
    {
        switch(equippedThrowableType)
        {
            case Throwable.ThrowableType.Grenade:
                return grenadePrefab;
            case Throwable.ThrowableType.Smoke:
                return smokeGrenadePrefab;
        }
        return new();
    }

    public void PickupWeapon(GameObject pickedupWeapon)
    {
        AddWeaponIntoActiveSlot(pickedupWeapon);
    }

    internal void PickupAmmo(AmmoBox ammo)
    {
        switch(ammo.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammo.ammoAmount;
                break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammo.ammoAmount;
                break;
        }
    }

    public void PickupThrowable(Throwable hoveredThrowable)
    {
        switch(hoveredThrowable.throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                PickupThrowableAsLethal(Throwable.ThrowableType.Grenade);
                print("detected grenade");
                break;
            case Throwable.ThrowableType.Smoke:
                PickupThrowableAsTactical(Throwable.ThrowableType.Smoke);
                print("detected smoke");
                break;
        }
    }

    private void PickupThrowableAsLethal(Throwable.ThrowableType lethal)
    {
        if(equippedLethalType == lethal || equippedLethalType == Throwable.ThrowableType.None)
        {
            equippedLethalType = lethal;
            if(lethalsCount < maxLethals)
            {
                lethalsCount += 1;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowables();
                print("lethal added");
            }
            else 
            {
                print("Lethals limit reached");
            }
        }
        else 
        {
            //cannot pickup
            //option to switch
        }
    }

    private void PickupThrowableAsTactical(Throwable.ThrowableType tactical)
    {
        if(equippedTacticalType == tactical || equippedTacticalType == Throwable.ThrowableType.None)
        {
            equippedTacticalType = tactical;
            if(tacticalCount < maxTacticals)
            {
                tacticalCount += 1;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowables();
                print("tactical added");
            }
            else 
            {
                print("tacticals limit reached");
            }
        }
        else 
        {
            //cannot pickup
            //option to switch
        }
    }


    private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    {
        DropCurrentWeapon(pickedupWeapon);
        print("AddWeaponIntoActiveSlot is working");
        pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);
        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();
        pickedupWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);
        pickedupWeapon.transform.localScale = new Vector3(weapon.spawnScale.x, weapon.spawnScale.y, weapon.spawnScale.z);
        pickedupWeapon.GetComponent<Animator>().enabled = true;
        weapon.isActiveWeapon = true;
    }

    private void DropCurrentWeapon(GameObject pickedupWeapon)
    {
        if(activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;
            weaponToDrop.GetComponent<Weapon>().isActiveWeapon = false;
            weaponToDrop.GetComponent<Animator>().enabled = false;
            weaponToDrop.transform.SetParent(pickedupWeapon.transform.parent);
            weaponToDrop.transform.localPosition = pickedupWeapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickedupWeapon.transform.localRotation;
        }
    }
    public void SwitchActiveSlot(int slotNumber)
    {
        if(activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;
        }
        activeWeaponSlot = weaponSlots[slotNumber];
        if(activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            if(newWeapon.GetComponent<Outline>().enabled == true)
            {
                newWeapon.GetComponent<Outline>().enabled = false;
                newWeapon.GetComponent<Animator>().enabled = true;
            }
            newWeapon.isActiveWeapon = true;
        }
    }

    public int CheckAmmoLeftFor(Weapon.WeaponModel thisweaponModel)
    {
        switch(thisweaponModel)
        {
            case Weapon.WeaponModel.M4A1:
                return totalRifleAmmo;
            case Weapon.WeaponModel.Skorpion:
                return totalPistolAmmo;
            default:
                return 0;
        }
    }

    internal void DecreaseTotalAmmo(int bulletsToDecrease, Weapon.WeaponModel thisweaponModel)
    {
        switch(thisweaponModel)
        {
            case Weapon.WeaponModel.M4A1:
                totalRifleAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.Skorpion:
                totalPistolAmmo -= bulletsToDecrease;
                break;

        }
    }
}
