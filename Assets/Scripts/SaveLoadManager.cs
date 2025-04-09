using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; set; }
    public string highScoreKey = "BesWaveSavedValue";

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
        DontDestroyOnLoad(this);
    }

    

    public void SaveHighScore(int score)
    {
        PlayerPrefs.SetInt(highScoreKey, score);
    }

    public int LoadHighScore()
    {
        if(PlayerPrefs.HasKey(highScoreKey))
        {
            return PlayerPrefs.GetInt(highScoreKey);
        }
        else 
        {
            return 0;
        }
    }
}
