using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }
    public AudioSource ShootingChannel;
    public AudioSource reloadingSoundM4;
    public AudioSource emptySoundM4;
    public AudioSource reloadingSoundSkorpion;
    public AudioClip M4A1Shot;
    public AudioClip SkorpionShot;

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

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch(weapon)
        {
            case WeaponModel.M4A1:
                ShootingChannel.PlayOneShot(M4A1Shot);
                break;
            case WeaponModel.Skorpion:
                ShootingChannel.PlayOneShot(SkorpionShot);
                break;
        }
    }
    public void PlayReloadSound(WeaponModel weapon)
    {
         switch(weapon)
        {
            case WeaponModel.M4A1:
                reloadingSoundM4.Play();
                break;
            case WeaponModel.Skorpion:
                reloadingSoundSkorpion.Play();
                break;
        }
    }
}
