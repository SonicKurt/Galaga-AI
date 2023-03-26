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

    // Player instance.
    public GameObject player;

    // Initial lives for the player to beign with.
    public int initialLives;

    private int playerCount;
    private int currentPlayer;
    private int currentStage;

    private int[] scores;
    private int highScore;

    private int[] lives;

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

    /// <summary>
    /// Switches the game state of where the game logic is currently at.
    /// </summary>
    /// <param name="newState">The new game state to be in.</param>
    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (state) {
            case GameState.PlayerSelect:
                // Start the game with player one being up first.
                MenuManager.Instance.UpdateHighScoreTextField(highScore);
                currentPlayer = 1;
                currentStage = 1;
                break;
            case GameState.DisplayStageText:
                // If scores and lives does not exist for the players,
                // initialize them with values to begin their session.
                if (scores == null) {
                    InitScores();
                }

                if (lives == null) {
                    InitLives();
                    Debug.Log(lives[currentPlayer - 1]);
                }

                // Displays the current stage that the player is on.
                // The MenuManager should consist of the text fields to display
                // the current stage.
                StartCoroutine(DisplayStage());
                break;
            case GameState.LoadEnemies:
                GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");
                SpawnerController spawnerController = spawner.GetComponent<SpawnerController>();

                // Loads the aliens into their proper positions.    
                spawnerController.SpawnAliens();
                break;
            case GameState.EnemiesFall:
                // TODO: Implement the logic to let the aliens start falling.



                break;
            case GameState.SwitchPlayer:
                // TODO: Implement the ability to switch players if player
                // count is two.

                break;
            case GameState.GameOver:
                // TODO: Set the Game Over panel active within the Canvas.



                //PlayerPrefs.SetInt("High Score");
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Initialize the scores to start from zero.
    /// </summary>
    private void InitScores() {
        scores = new int[playerCount];
        for (int i = 0; i < playerCount; i++) {
            scores[i] = 0;
        }
    }

    /// <summary>
    /// Initialize the lives to start from the initial live counter.
    /// </summary>
    private void InitLives() {
        lives = new int[playerCount];
        for (int i = 0; i < playerCount; i++) {
            lives[i] = initialLives;
        }
    }

    /// <summary>
    /// Reset the score for the current player.
    /// </summary>
    private void ResetScore() {
        MenuManager.Instance.UpdateScoreTextFields(0, currentPlayer);
    }

    /// <summary>
    /// Displays the current stage text. If the player is beginning the game,
    /// it will display the current player that is currently up.
    ///
    /// When the game is in two player mode, it will display the current player text
    /// to indicate that player is ready to play.
    ///
    /// It will display the current stage value once you begin the next stage.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayStage() {
        yield return new WaitForSeconds(1f);

        if (currentStage == 1) {
            MenuManager.Instance.UpdateCurrentPlayerTextField(currentPlayer);
            MenuManager.Instance.currentPlayerText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(3f);

        MenuManager.Instance.UpdateCurrentStageTextField(currentStage);

        SpawnPlayer();

        MenuManager.Instance.currentStageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        MenuManager.Instance.currentStageText.gameObject.SetActive(false);
        MenuManager.Instance.currentPlayerText.gameObject.SetActive(false);
        UpdateGameState(GameState.LoadEnemies);
    }

    /// <summary>
    /// Spawns a player instance.
    /// </summary>
    private void SpawnPlayer() {
        Vector3 spawnPos = new Vector3(-1.1f, 0f, -4f);
        Instantiate(player, spawnPos, Quaternion.identity);
    }
}

/// <summary>
/// The states to keep the game flow moving properly.
/// </summary>
public enum GameState
{
    PlayerSelect,
    DisplayStageText,
    LoadEnemies,
    EnemiesFall,
    SwitchPlayer,
    GameOver
}

/// <summary>
/// The type of enemies.
/// </summary>
public enum EnemyType
{
    Goei,
    Stringer,
    BossGalaga
}

/// <summary>
/// Loading phase for the aliens to load in.
/// </summary>
public enum LoadEnemyState
{
    Phase1,
    Phase2,
    Phase3,
    Phase4,
    Phase5,
    Done
}