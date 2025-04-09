using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GlobalReferences : MonoBehaviour
{
    public static GlobalReferences Instance { get; set; }
    public GameObject bulletImpactEffectPrefab;
    public GameObject grenadeExplosionEffect;
    public GameObject smokeGrenadeEffect;
    public GameObject bloodSprayEffect;
    public int roundNumber;

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
}
