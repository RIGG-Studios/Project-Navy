using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    public static EndGameUI Instance;
    
    [SerializeField] private GameObject gameOverUI;


    private void Awake()
    {
        Instance = this;
    }

    public void ShowGameOverScreen()
    {
        gameOverUI.SetActive(true);
    }

    public void Spectate()
    {
        GameManager.Instance.Spectate();
    }

    public void Exit()
    {
        GameManager.Instance.ExitMatch();
    }
}
