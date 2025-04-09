using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
public class Zombie : MonoBehaviour
{
   public ZombieHand zombieHand;
   public int zombieDamage;
    
   private void Start()
   {
      zombieHand.damage = zombieDamage;
   }
}
