/**********************************************************
 * Game Manager
 * 
 * Summary: Manages the game's flow.
 * 
 * Author: Kurt Campbell
 * Created: 19 March 2023
 * 
 * Copyright Cedarville University, Kurt Campbell, Jackson Isenhower,
 * Donald Osborn.
 * All rights reserved.
 *********************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState state;

    // Player instance.
    public GameObject player;

    // Initial lives for the player to beign with.
    public int initialLives;

    private int playerCount;

    [SerializeField]
    private int currentPlayer;

    private int[] currentStage;

    private int[] scores;
    private int highScore;

    private int[] lives;

    private SpawnerController spawnerController;

    private int[,] enemyNumberRanges;

    // The amount of aliens to attack one time.
    public int aliensAttacking;

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

                enemyNumberRanges = new int[6 , 2];

                // Stringer number ranges
                enemyNumberRanges[0, 0] = 0;
                enemyNumberRanges[0, 1] = 3;
                enemyNumberRanges[1, 0] = 24;
                enemyNumberRanges[1, 1] = 39;

                // Boss Galaga number ranges
                enemyNumberRanges[2, 0] = 8;
                enemyNumberRanges[2, 1] = 11;

                // Goei number ranges
                enemyNumberRanges[3, 0] = 4;
                enemyNumberRanges[3, 1] = 7;
                enemyNumberRanges[4, 0] = 12;
                enemyNumberRanges[4, 1] = 15;
                enemyNumberRanges[5, 0] = 16;
                enemyNumberRanges[5, 1] = 23;

                currentPlayer = 1; 
                break;
            case GameState.DisplayStageText:
                // If scores and lives does not exist for the players,
                // initialize them with values to begin their session.
                if (scores == null) {
                    InitScores();
                }

                if (lives == null) {
                    InitLives();
                    MenuManager.Instance.UpdateLiveCounterText(lives[currentPlayer - 1]);
                }

                // Initialize the stage counters.
                InitStageCounters();

                // Update the stage counter text field.
                MenuManager.Instance.UpdateStageCounterText(currentStage[currentPlayer - 1]);

                // Displays the current stage that the player is on.
                // The MenuManager should consist of the text fields to display
                // the current stage.
                StartCoroutine(DisplayStage());
                break;
            case GameState.LoadEnemies:
                GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");
                spawnerController = spawner.GetComponent<SpawnerController>();

                // Loads the aliens into their proper positions.    
                spawnerController.SpawnAliens();
                break;
            case GameState.EnemiesAttack:
                // TODO: Implement the logic to let the aliens start falling.

                StartCoroutine(AlienAttack());


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
    /// Initialize the stage counters for each player.
    /// </summary>
    private void InitStageCounters() {
        currentStage = new int[playerCount];

        for (int i = 0; i < playerCount; i++) {
            currentStage[i] = 1;
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

        if (currentStage[currentPlayer - 1] == 1) {
            MenuManager.Instance.UpdateCurrentPlayerTextField(currentPlayer);
            MenuManager.Instance.currentPlayerText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(3f);

        MenuManager.Instance.UpdateCurrentStageTextField(currentStage[currentPlayer - 1]);

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

    /// <summary>
    /// Let the aliens start attacking the player.
    /// </summary>
    /// <returns>
    /// The amount of seconds before letting another alien
    /// attack.
    /// </returns>
    private IEnumerator AlienAttack() {
        List<GameObject> aliens = spawnerController.Aliens;

        Random randomizer = new Random();
        int alienIndex = 0;
        int resetAlien = randomizer.Next(1, 3);
        EnemyAttackState enemyAttackState = EnemyAttackState.Phase1;

        List<GameObject> currAliensAttacking = new List<GameObject>();

        while (enemyAttackState != EnemyAttackState.Done) {
            for (int i = 0; i < aliensAttacking; i++) {
                // Let a Goei or Stringer attack.
                if (enemyAttackState == EnemyAttackState.Phase1) {
                    EnemyType alienType = (EnemyType)randomizer.Next(0, 2);
                    int range = 0;

                    if (alienType == EnemyType.Goei) {
                        range = randomizer.Next(3, 6);
                    } else {
                        range = randomizer.Next(0, 2);
                    }

                    alienIndex = randomizer.Next(enemyNumberRanges[range, 0], enemyNumberRanges[range, 1] + 1);

                    GameObject alien = aliens[alienIndex];

                    AlienController alienController = alien.GetComponent<AlienController>();
                    alienController.Attack = true;
                    alienController.ResetToPosition = (resetAlien == i + 1);
                    currAliensAttacking.Add(alien);
                }

                // Let a Goei, Stringer, or Boss Galaga attack.
                if (enemyAttackState == EnemyAttackState.Phase2 && i == 0) {
                    EnemyType alienType = (EnemyType)randomizer.Next(0, 3);
                    int range = 0;

                    if (alienType == EnemyType.Goei) {
                        range = randomizer.Next(3, 6);
                    } else if (alienType == EnemyType.Stringer) {
                        range = randomizer.Next(0, 2);
                    } else {
                        range = 2;
                    }

                    alienIndex = randomizer.Next(enemyNumberRanges[range, 0], enemyNumberRanges[range, 1] + 1);

                    GameObject alien = aliens[alienIndex];

                    AlienController alienController = alien.GetComponent<AlienController>();
                    alienController.Attack = true;
                    alienController.ResetToPosition = (resetAlien == i + 1);
                    currAliensAttacking.Add(alien);
                }
            }

            // Timeout before letting another alien attack.
            yield return new WaitForSeconds(11f);

            int numOfAliensAttacking = currAliensAttacking.Count;

            while (currAliensAttacking.Count != 0) {
                AlienController alienController = currAliensAttacking[--numOfAliensAttacking].GetComponent<AlienController>();
                alienController.ResetToPosition = true;
                currAliensAttacking.RemoveAt(numOfAliensAttacking);
            }

            // TODO: Modify this condition once the ability to shoot down
            // enemies is a thing. Make sure that all the aliens are dead
            // and set the attack state to be done.
            if (enemyAttackState == EnemyAttackState.Phase1) {
                enemyAttackState = EnemyAttackState.Phase2;
            } else {
                enemyAttackState = EnemyAttackState.Phase1;
            }
        }
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
    EnemiesAttack,
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

public enum EnemyAttackState {
    Phase1,
    Phase2,
    Done
}