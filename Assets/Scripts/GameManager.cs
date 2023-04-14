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
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // The current state of the game.
    public GameState state;

    // Player instance.
    public GameObject player;

    // Initial lives for the player to beign with.
    public int initialLives;

    // The amount of players that are in the current session.
    private int playerCount;

    // The current player playing.
    [SerializeField]
    private int currentPlayer;

    // The current stage that each player is on.
    private int[] currentStage;

    private int[] scores;
    private int highScore;

    private int[] lives;

    private SpawnerController spawnerController;

    private int[,] enemyNumberRanges;

    // The amount of aliens to attack one time.
    public int aliensAttacking;

    private List<GameObject> currAliensAttacking;

    // Check to see if the current player is dead.
    public bool PlayerDead { get; set; }

    public bool Training { get; set; }

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
                PlayerDead = false;
                break;
            case GameState.DisplayStageText:
                // If scores and lives does not exist for the players,
                // initialize them with values to begin their session.
                if (scores == null) {
                    InitScores();
                }

                if (lives == null) {
                    InitLives();

                    // Initialize the stage counters.
                    InitStageCounters();
                }

                // Update the lives text field.
                UpdateLivesTextField();

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
                
                int alienCount = spawnerController.Aliens.Count;

                currAliensAttacking = new List<GameObject>();
                Debug.Log(currentPlayer + "'s Aliens Attack!");
                StartCoroutine(AlienAttack());

                break;
            case GameState.SwitchPlayer:
                ClearAliens();

                if (currentPlayer == 1) {
                    currentPlayer++;
                } else {
                    currentPlayer--;
                }

                if (lives[currentPlayer - 1] > 0) {
                    UpdateGameState(GameState.DisplayStageText);
                } else {
                    UpdateGameState(GameState.GameOver);
                }

                break;
            case GameState.GameOver:
                // TODO: Set the Game Over panel active within the Canvas.

                //PlayerPrefs.SetInt("High Score");

                Destroy(MenuManager.Instance.canvas.gameObject);
                Destroy(MenuManager.Instance.gameObject);
                Destroy(this.gameObject);

                SceneManager.LoadScene(0);
                
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Updates the current player's score based on the destoryed alien.
    /// </summary>
    /// <param name="alienType">The type of alien that was destroyed</param>
    /// <param name="attacking">The state whether or not the alien was attacking.</param>
    public void UpdateScore(EnemyType alienType, bool attacking) {
        switch (alienType) {
            case EnemyType.Goei:
                if (attacking) {
                    scores[currentPlayer - 1] += 160;
                } else {
                    scores[currentPlayer - 1] += 80;
                }
                break;
            case EnemyType.Stringer:
                if (attacking) {
                    scores[currentPlayer - 1] += 100;
                } else {
                    scores[currentPlayer - 1] += 50;
                }
                break;
            case EnemyType.BossGalaga:
                if (attacking) {
                    scores[currentPlayer - 1] += 400;
                } else {
                    scores[currentPlayer - 1] += 150;
                }
                break;
        }


        MenuManager.Instance.UpdateScoreTextFields(scores[currentPlayer - 1], currentPlayer);
    }

    /// <summary>
    /// Removes the alien object from the attacking list.
    /// </summary>
    /// <param name="alien"></param>
    public void RemoveAlienAttacking(GameObject alien) {
        currAliensAttacking.Remove(alien);
    }

    /// <summary>
    /// Removes the alien from the current player's grid.
    /// </summary>
    /// <param name="alien"></param>
    public void RemoveAlien(GameObject alien) {
        AlienController alienController = alien.GetComponent<AlienController>();
        
        //playerAlienGrids[currentPlayer - 1].Remove(alien);
        spawnerController.Aliens.Remove(alien);
        
        if (alienController.Attack) {
            currAliensAttacking.Remove(alien);
        }
    }

    public int getCurrentStage() {
        return currentStage[currentPlayer - 1];
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

        bool playerSpawned = false;

        if (currentStage[currentPlayer - 1] > 1) {
            MenuManager.Instance.UpdateCurrentStageTextField(currentStage[currentPlayer - 1]);
        } else if (currentStage[currentPlayer - 1] == 1 && lives[currentPlayer - 1] == initialLives) {
            MenuManager.Instance.UpdateCurrentPlayerTextField(currentPlayer);
            MenuManager.Instance.currentPlayerText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            MenuManager.Instance.UpdateCurrentStageTextField(currentStage[currentPlayer - 1]);
            SpawnPlayer();
            playerSpawned = true;
        } 
        
        if (PlayerDead) {
            PlayerDead = false;
            // Displays the ready state text.
            if (lives.Length == 2) {
                MenuManager.Instance.UpdateCurrentPlayerTextField(currentPlayer);
                MenuManager.Instance.currentPlayerText.gameObject.SetActive(true);
            }
            
            MenuManager.Instance.UpdateCurrentStageTextField();
            if (!playerSpawned) {
                SpawnPlayer();
            }
        }

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
        Instantiate(player, spawnPos, Quaternion.Euler(-90, 90, 0));
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
        
        // If all of the aliens have been destroyed, we will increase
        // the stage counter and proceed to the next stage.
        Random randomizer = new Random();
        int alienIndex = 0;
        int resetAlien = randomizer.Next(1, 3);
        EnemyAttackState enemyAttackState = EnemyAttackState.Attack;

        if (aliens.Count == 0) {
            enemyAttackState = EnemyAttackState.Done;
        }

        while (enemyAttackState != EnemyAttackState.Done) {
            for (int i = 0; i < aliensAttacking; i++) {
                alienIndex = randomizer.Next(0, aliens.Count);

                if (aliens.Count != 0) {
                    GameObject alien = aliens[alienIndex];

                    AlienController alienController = alien.GetComponent<AlienController>();
                    alienController.Attack = true;

                    alienController.ResetToPosition = (resetAlien == i + 1);
                    currAliensAttacking.Add(alien);
                }
            }

            
            if (PlayerDead) {
                lives[currentPlayer - 1]--;

                // If there are two players playing, we want to switch
                // to the current player.
                if (playerCount == 2) {
                    UpdateGameState(GameState.SwitchPlayer);
                    yield break;
                }

                // If the current single player has lives, we
                // must reset the alien positions, update the live counter text
                // field, and update the game state to display the stage.
                if (lives[currentPlayer - 1] > 0) {
                    // Reset the aliens to go back to their grid position.
                    //ResetAliens(currAliensAttacking);
                    
                    ClearAliens();
                    UpdateLivesTextField();

                    if (Training) {
                        UpdateGameState(GameState.LoadEnemies);
                    } else {
                        UpdateGameState(GameState.DisplayStageText);
                    }

                    yield break;
                }

                UpdateGameState(GameState.GameOver);
                yield break;
            } 
            
            if (aliens.Count == 0) {
                enemyAttackState = EnemyAttackState.Done;
            } else {
                // Timeout before letting another alien attack.
                yield return new WaitForSeconds(3f);
                ResetAliens(currAliensAttacking);
            }
        }

        // When training, it should switch to the next stage.
        if (Training) {
            currentStage[currentPlayer - 1]++;
            UpdateGameState(GameState.LoadEnemies);
            yield break;
        }

        if (aliens.Count == 0) {
            currentStage[currentPlayer - 1]++;
            UpdateGameState(GameState.DisplayStageText);
            yield break;
        }
    }

    /// <summary>
    /// Clears the entire alien grid from the grid object.
    /// </summary>
    private void ClearAliens() {
        spawnerController.ClearGrid();
    }

    private void ResetAliens(List<GameObject> currAliensAttacking) {
        int numOfAliensAttacking = currAliensAttacking.Count;

        while (currAliensAttacking.Count != 0) {
            AlienController alienController = currAliensAttacking[--numOfAliensAttacking].GetComponent<AlienController>();
            alienController.ResetToPosition = true;
            currAliensAttacking.RemoveAt(numOfAliensAttacking);
        }
    }

    /// <summary>
    /// Update the live counter text field for the current player.
    /// </summary>
    private void UpdateLivesTextField() {
        MenuManager.Instance.UpdateLiveCounterText(lives[currentPlayer - 1]);
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

/// <summary>
/// Attack phases to which aliens can attack for the current timeframe.
/// </summary>
public enum EnemyAttackState {
    Attack,
    Done
}

public enum BulletType {
    Player,
    Alien
}