using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class MainMenu : MonoBehaviour
{

    public TMP_Text highScoreUI;
    string nextGameScene = "SampleScene";
    public AudioSource MenuMusic;
    public AudioClip MenuSong;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MenuMusic.PlayOneShot(MenuSong);
        int highScore = SaveLoadManager.Instance.LoadHighScore();
        highScoreUI.text = $"Top Wave Survived: {highScore}";
    }

    public void StartNewGame()
    {
        MenuMusic.Stop();
        SceneManager.LoadScene(nextGameScene);
    }

    public void ExitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
