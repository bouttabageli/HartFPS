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
            totalAmmoUI.text = $"{activeWeapon.magazineSize / activeWeapon.bulletsPerBurst}";
            activeWeapon.WeaponModel model = activeWeapon.thisweaponModel;
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
        }
    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        switch(model)
        {
            case Weapon.WeaponModel.M4A1:
                return Instantiate(Resources.Load<GameObject>("M4A1_Weapon")).GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.Skorpion:
                return Instantiate(Resources.Load<GameObject>("Skorpion_Weapon")).GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        switch(model)
        {
            case Weapon.WeaponModel.M4A1:
                return Instantiate(Resources.Load<GameObject>("Rifle_Ammo")).GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.Skorpion:
                return Instantiate(Resources.Load<GameObject>("Pistol_Ammo")).GetComponent<SpriteRenderer>().sprite;
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




}
