using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class AmmoBox : MonoBehaviour
{
    public int ammoAmount = 200;
    public AmmoType ammoType;
    public bool Consumable;

    public enum AmmoType
    {
        RifleAmmo,
        PistolAmmo
    }
}
