using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class HUDManager : MonoBehaviour
{
     public static HUDManager Instance { get; set; }

     [Header("Ammo")]
     public TextMeshProUGUI magazineAmmoUI;
     public TextMeshProUGUI totalAmmoUI;
     public Image ammoTypeUI;

     [Header("Weapon")]
     public Image activeWeaponUI;
     public Image unActiveWeaponUI;

     [Header("Throwables")]
     public Image lethalUI;
     public TextMeshProUGUI lethalAmountUI;
     public Image tacticalUI;
     public TextMeshProUGUI tacticalAmountUI;

     public Sprite emptySlot;
     public Sprite greySlot;

     public GameObject crosshair;

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

    private void Update()
    {
        Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unActiveWeapon = GetUnActiveWeaponSlot().GetComponentInChildren<Weapon>();
        if(activeWeapon)
        {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft / activeWeapon.bulletsPerBurst}";
            totalAmmoUI.text = $"{WeaponManager.Instance.CheckAmmoLeftFor(activeWeapon.thisweaponModel)}";
            Weapon.WeaponModel model = activeWeapon.thisweaponModel;
            ammoTypeUI.sprite = GetAmmoSprite(model);
            activeWeaponUI.sprite = GetWeaponSprite(model);
            if(unActiveWeapon)
            {
                unActiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.thisweaponModel);
            }
        }
        else 
        {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";
            ammoTypeUI.sprite = emptySlot;
            activeWeaponUI.sprite = emptySlot;
            unActiveWeaponUI.sprite = emptySlot;
        }

        if(WeaponManager.Instance.lethalsCount <= 0)
        {
            lethalUI.sprite = greySlot;
        }
        if(WeaponManager.Instance.tacticalCount <= 0)
        {
            tacticalUI.sprite = greySlot;
        }
    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        switch(model)
        {
            case Weapon.WeaponModel.M4A1:
                return Resources.Load<GameObject>("M4A1_Weapon").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.Skorpion:
                return Resources.Load<GameObject>("Skorpion_Weapon").GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        switch(model)
        {
            case Weapon.WeaponModel.M4A1:
                return Resources.Load<GameObject>("Rifle_Ammo").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.Skorpion:
                return Resources.Load<GameObject>("Pistol_Ammo").GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
    }

    private GameObject GetUnActiveWeaponSlot()
    {
        foreach(GameObject weaponSlot in WeaponManager.Instance.weaponSlots)
        {
            if(weaponSlot != WeaponManager.Instance.activeWeaponSlot)
            {
                return weaponSlot;
            }
        }
        //this will never happen but we need to return something
        return null;
    }

    public void UpdateThrowables()
    {
        lethalAmountUI.text = $"{WeaponManager.Instance.lethalsCount}";
        tacticalAmountUI.text = $"{WeaponManager.Instance.tacticalCount}";
        switch(WeaponManager.Instance.equippedLethalType)
        {
            case Throwable.ThrowableType.Grenade:
                lethalUI.sprite = Resources.Load<GameObject>("Grenade").GetComponent<SpriteRenderer>().sprite;
                break;
        }

        switch(WeaponManager.Instance.equippedTacticalType)
        {
            case Throwable.ThrowableType.Smoke:
                tacticalUI.sprite = Resources.Load<GameObject>("Smoke").GetComponent<SpriteRenderer>().sprite;
                break;
        }
    }


}
