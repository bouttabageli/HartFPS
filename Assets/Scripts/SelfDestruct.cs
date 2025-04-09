using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class SelfDestruct : MonoBehaviour
{
    public float timeToDestruction;
    
    void Start()
    {
        StartCoroutine(DestroySelf(timeToDestruction));
    }

    private IEnumerator DestroySelf(float timeToDestruction)
    {
        yield return new WaitForSeconds(timeToDestruction);
        Destroy(gameObject);
    }

}
