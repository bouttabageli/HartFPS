using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ZombieSpawnController : MonoBehaviour
{
    public int initialZombiesPerWave = 5;
    public int currentZombiesPerWave;

    public float spawnDelay = 0.5f; //delay between spawning each zombie in a wave

    public int currentWave = 0;
    public float waveCooldown = 10.0f; //time in seconds between waves

    public bool isCooldown;
    public float cooldownCounter = 0; //UI and testing purposes

    public List<Enemy> currentZombiesAlive;

    public GameObject zombiePrefab;

    public TextMeshProUGUI RoundOverTextUI;
    public TextMeshProUGUI RoundOverNumberUI;
    public TextMeshProUGUI RoundNumberUI;

    private void Start()
    {
        currentZombiesPerWave = initialZombiesPerWave;
        GlobalReferences.Instance.roundNumber = currentWave;
        StartNextWave();
    }

    private void StartNextWave()
    {
        currentZombiesAlive.Clear();
        currentWave += 1;
        GlobalReferences.Instance.roundNumber = currentWave;
        RoundNumberUI.text = $"{currentWave}";
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for(int i = 0; i < currentZombiesPerWave; i++)
        {
            //Generate a random offset within a specified range
            Vector3 spawnOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
            Vector3 spawnPosition = transform.position + spawnOffset;
            //Instantiate the zombie
            var zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
            //Get enemy script
            Enemy enemyScript = zombie.GetComponent<Enemy>();
            //Track this zombie
            currentZombiesAlive.Add(enemyScript);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void Update()
    {
        //Get all dead bodies
        List<Enemy> zombiesToRemove = new List<Enemy>();
        foreach(Enemy zombie in currentZombiesAlive)
        {
            if(zombie.isDead)
            {
                zombiesToRemove.Add(zombie);
            }
        }

        //Actually remove all dead bodies
        foreach(Enemy zombie in zombiesToRemove)
        {
            currentZombiesAlive.Remove(zombie);
        }

        zombiesToRemove.Clear();

        //Start Cooldown if all zombies are dead
        if(currentZombiesAlive.Count == 0 && isCooldown == false)
        {
            RoundOverNumberUI.text = $"{currentWave + 1}";
            //Start cooldown
            StartCoroutine(WaveCooldown());
        }

        //Run the cooldown counter
        if(isCooldown)
        {
            cooldownCounter -= Time.deltaTime;
        }
        else 
        {
            cooldownCounter = waveCooldown;
        }

    }

    private IEnumerator WaveCooldown()
    {
        isCooldown = true;
        RoundOverNumberUI.gameObject.SetActive(true);
        RoundOverTextUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(waveCooldown);
        RoundOverNumberUI.gameObject.SetActive(false);
        RoundOverTextUI.gameObject.SetActive(false);
        isCooldown = false;
        currentZombiesPerWave *= 2;
        StartNextWave();
    }   
    
}
