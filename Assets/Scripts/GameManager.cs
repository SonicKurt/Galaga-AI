/**********************************************************
 * Game Manager
 * 
 * Summary: Manages the game's flow.
 * 
 * Author: Kurt Campbell
 * Date: 19 March 2023
 * 
 * Copyright Cedarville University, Kurt Campbell, Jackson Isenhower,
 * Donald Osborn.
 * All rights reserved.
 *********************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState state;

    private int playerCount;
    private int currentPlayer;
    private int currentStage;

    private int[] scores;
    private int highScore;

    public int PlayerCount {
        get {
            return playerCount;
        }

        set {
            playerCount = value;
        }
    }

    private void Awake()
    {
        Instance = this;
        highScore = PlayerPrefs.GetInt("High Score", 30000);
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        UpdateGameState(GameState.PlayerSelect);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (state)
        {
            case GameState.PlayerSelect:
                MenuManager.Instance.UpdateHighScoreTextField(highScore);
                currentPlayer = 1;
                currentStage = 1;
                break;
            case GameState.DisplayStageText:
                if (scores == null) {
                    InitScores();
                }

                if (scores[currentPlayer - 1] == 0) {
                    ResetScore();
                }

                // Displays the current stage that the player is on.
                // The MenuManager should consist of the text fields to display
                // the current stage.
                StartCoroutine(DisplayStage());
                break;
            case GameState.LoadEnemies:
                break;
            case GameState.EnemiesFall:
                break;
            case GameState.SwitchPlayer:
                break;
            case GameState.GameOver:

                
                //PlayerPrefs.SetInt("High Score");
                break;
            default:
                break;
        }
    }

    private void InitScores() {
        scores = new int[playerCount];
        for (int i = 0; i < playerCount; i++) {
            scores[i] = 0;
        }
    }

    private void ResetScore() {
        MenuManager.Instance.UpdateScoreTextFields(0, currentPlayer);
    }

    private IEnumerator DisplayStage() {
        yield return new WaitForSeconds(1f);

        if (currentStage == 1) {
            MenuManager.Instance.UpdateCurrentPlayerTextField(currentPlayer);
            MenuManager.Instance.currentPlayerText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(3f);

        MenuManager.Instance.UpdateCurrentStageTextField(currentStage);
        
        MenuManager.Instance.currentStageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        MenuManager.Instance.currentStageText.gameObject.SetActive(false);
        MenuManager.Instance.currentPlayerText.gameObject.SetActive(false);
        state = GameState.LoadEnemies;
    }
}

public enum GameState
{
    PlayerSelect,
    DisplayStageText,
    LoadEnemies,
    EnemiesFall,
    SwitchPlayer,
    GameOver
}

public enum EnemyType
{
    Goei,
    Stringer,
    BossGalaga
}

public enum LoadEnemyState
{
    Phase1,
    Phase2,
    Phase3,
    Phase4,
    Phase5,
    Done
}