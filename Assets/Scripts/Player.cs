using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class Player : MonoBehaviour
{
    public int HP = 100;
    public GameObject bloodyScreen;
    public TextMeshProUGUI playerHealthUI;
    public GameObject gameOverUI;
    public bool isDead;

    private void Start()
    {
        playerHealthUI.text = $"Health: {HP}";
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        if(HP <= 0)
        {
            print("Player dead");
            PlayerDead();
            isDead = true;
        }
        else 
        {
            print("Player hurt");
            StartCoroutine(BloodyScreenEffect());
            playerHealthUI.text = $"Health: {HP}";
            SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerHurt);
        }
    }

    private IEnumerator BloodyScreenEffect()
    {
        if(bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(true);
        }

        var image = bloodyScreen.GetComponentInChildren<Image>();
 
        // Set the initial alpha value to 1 (fully visible).
        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;
 
        float duration = 3f;
        float elapsedTime = 0f;
 
        while (elapsedTime < duration)
        {
            // Calculate the new alpha value using Lerp.
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
 
            // Update the color with the new alpha value.
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;
 
            // Increment the elapsed time.
            elapsedTime += Time.deltaTime;
 
            yield return null; ; // Wait for the next frame.
        }

        if(bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ZombieHand"))
        {
            if(!isDead)
            {
                TakeDamage(other.gameObject.GetComponent<ZombieHand>().damage);
            }
        }

    }

    private void PlayerDead()
    {
        SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerDie);
        GetComponent<MouseMovement>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        playerHealthUI.gameObject.SetActive(false);

        //Dying animation
        GetComponentInChildren<Animator>().enabled = true;
        GetComponent<ScreenFader>().StartFade();
        StartCoroutine(ShowGameOverUI());
    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1f);
        gameOverUI.gameObject.SetActive(true);

        int roundSurvived = GlobalReferences.Instance.roundNumber;

        if(roundSurvived-1 > SaveLoadManager.Instance.LoadHighScore())
        {
            SaveLoadManager.Instance.SaveHighScore(roundSurvived-1);
        }
        StartCoroutine(ReturnToMenu());
    }

    private IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MainMenu");
    }
}
