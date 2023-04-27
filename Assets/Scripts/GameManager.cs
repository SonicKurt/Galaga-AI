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
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // The current state of the game.
    public GameState state;

    // Player instance.
    public GameObject player;

    // Player Agent instance.
    public GameObject playerAgent;

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

    private bool stageCompleted;

    private int[,] enemyNumberRanges;

    // The amount of aliens to attack one time.
    public int aliensAttacking;

    private List<GameObject> currAliensAttacking;

    // Check to see if the current player is dead.
    public bool PlayerDead { get; set; }

    public bool training;

    public bool Spawning { get; set; }

    public int PlayerCount {
        get {
            return playerCount;
        }

        set {
            playerCount = value;
        }
    }

    private AudioSource alienDeathSoundEffect;

    private void Awake()
    {
        Instance = this;
        highScore = PlayerPrefs.GetInt("High Score", 30000);

        if (training) {
            player.SetActive(false);
            playerAgent.SetActive(true);
        } else {
            player.SetActive(true);
            playerAgent.SetActive(false);
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        alienDeathSoundEffect = GetComponent<AudioSource>();
        stageCompleted = false;

        if (training) {
            playerCount = 1;
            currentPlayer = 1;
            PlayerDead = false;
            Spawning = false;
        } else {
            UpdateGameState(GameState.PlayerSelect);
        }
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

                // Updates the high score text field.
                MenuManager.Instance.UpdateHighScoreTextField(highScore);

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
                if (!training) {
                    StartCoroutine(DisplayStage());
                } 

                break;
            case GameState.LoadEnemies:
                GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");
                spawnerController = spawner.GetComponent<SpawnerController>();
                
                // Loads the aliens into their proper positions.
                if (!Spawning) {
                    spawnerController.SpawnAliens();
                }

                break;
            case GameState.EnemiesAttack:
                int alienCount = spawnerController.Aliens.Count;

                currAliensAttacking = new List<GameObject>();
                StartCoroutine(AlienAttack());

                break;
            case GameState.SwitchPlayer:
                //ClearAliens();

                if (currentPlayer == 1) {
                    currentPlayer++;
                } else {
                    currentPlayer--;
                }

                if (lives[currentPlayer - 1] > 0) {
                    UpdateGameState(GameState.DisplayStageText);
                } else {
                    // If the other player has zero lives, check the current player
                    // to see if the player still has lives to play.
                    if (currentPlayer == 1) {
                        currentPlayer++;
                    } else {
                        currentPlayer--;
                    }

                    if (lives[currentPlayer - 1] > 0) {
                        playerCount--;
                        UpdateGameState(GameState.DisplayStageText);
                    } else {
                        UpdateGameState(GameState.GameOver);
                    }
                }

                break;
            case GameState.GameOver:                
                // Sets the high score if it has been beaten.
                int bestScore = scores[0];

                if (playerCount == 2) {
                    if (bestScore < scores[1]) {
                        bestScore = scores[1];
                    }
                }

                if (bestScore > highScore) {
                    PlayerPrefs.SetInt("High Score", bestScore);
                }

                StartCoroutine(DisplayGameOver());

                break;
            case GameState.ResetEpisode:
                lives[currentPlayer - 1] = initialLives;
                ClearAliens();

                break;
            case GameState.PlayerDeath:
                spawnerController.StopAllCoroutines();
                Spawning = false;
                StopAllCoroutines();

                int alienCounter = spawnerController.Aliens.Count;
                if (alienCounter == 0) {
                    currentStage[currentPlayer - 1]++;
                    stageCompleted = true;
                }

                ClearAliens();

                if (playerCount == 2) {
                    UpdateGameState(GameState.SwitchPlayer);
                } else if (lives[currentPlayer - 1] > 0 || training) {
                    UpdateGameState(GameState.DisplayStageText);
                } else {
                    UpdateGameState(GameState.GameOver);
                }

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Toggles whether to have the player agent play the game.
    /// </summary>
    /// <param name="toggle">The toggle switch having the player agent enabled.</param>
    public void TogglePlayerAgent(bool toggle) {
        if (toggle) {
            playerAgent.SetActive(true);
            player.SetActive(false);
        } else {
            playerAgent.SetActive(false);
            player.SetActive(true);
        }
    }

    /// <summary>
    /// Plays the alien death sound if an alien died.
    /// </summary>
    public void PlayAlienDeathSound() {
        alienDeathSoundEffect.Play();
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

    /// <summary>
    /// Get the current stage for the current player.
    /// </summary>
    /// <returns></returns>
    public int getCurrentStage() {
        return currentStage[currentPlayer - 1];
    }

    /// <summary>
    /// Removes all the bullets from the gameplay scene.
    /// </summary>
    public void removeAllBulletsFromScene() {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

        if (bullets != null) {
            foreach (GameObject bullet in bullets) {
                Destroy(bullet);
            }
        }
    }

    /// <summary>
    /// Get the current amount of aliens that are in the grid.
    /// </summary>
    /// <returns>An integer that represents the current amount of aliens.</returns>
    public int getAlienCount() {
        GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");
        return spawner.transform.childCount;
    }

    /// <summary>
    /// Check to see if the grid contains no aliens.
    /// </summary>
    /// <returns>True if the grid is empty.</returns>
    public bool checkGridEmpty() {
        GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");
        return spawner.transform.childCount == 0;
    }

    /// <summary>
    /// Removes a life from the current player.
    /// </summary>
    public void LoseLife() {
        lives[currentPlayer - 1]--;
        UpdateLivesTextField();
    }

    /// <summary>
    /// Displays the Game Over text.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayGameOver() {
        GameObject gameOverSoundObj = GameObject.FindGameObjectWithTag("GameOverSound");
        AudioSource gameOverSound = gameOverSoundObj.GetComponent<AudioSource>();
        gameOverSound.Play();

        MenuManager.Instance.EnableGameOver();

        yield return new WaitForSeconds(13f);

        gameOverSound.Stop();

        // Destory this scene and restart to the main menu.
        Destroy(MenuManager.Instance.canvas.gameObject);
        Destroy(MenuManager.Instance.gameObject);
        Destroy(this.gameObject);

        SceneManager.LoadScene(0);
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

        GameObject firstStageObj = GameObject.FindGameObjectWithTag("FirstStage");
        AudioSource firstStageAudio = firstStageObj.GetComponent<AudioSource>();

        bool playerSpawned = false;

        if (currentStage[currentPlayer - 1] > 1) {
            MenuManager.Instance.UpdateCurrentStageTextField(currentStage[currentPlayer - 1]);
        } else if (currentStage[currentPlayer - 1] == 1 && lives[currentPlayer - 1] == initialLives) {
            firstStageAudio.Play();

            MenuManager.Instance.UpdateCurrentPlayerTextField(currentPlayer);
            MenuManager.Instance.currentPlayerText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            MenuManager.Instance.UpdateCurrentStageTextField(currentStage[currentPlayer - 1]);
            
            if (!training) {
                SpawnPlayer();
            }
            
            playerSpawned = true;
        } 
        
        if (PlayerDead) {
            PlayerDead = false;
            // Displays the ready state text.
            if (playerCount == 2) {
                MenuManager.Instance.UpdateCurrentPlayerTextField(currentPlayer);
                MenuManager.Instance.currentPlayerText.gameObject.SetActive(true);
            }

            if (!stageCompleted) {
                MenuManager.Instance.UpdateCurrentStageTextField();
            }

            if (!playerSpawned && !GameManager.Instance.training) {
                SpawnPlayer();
            }

            stageCompleted = false;
        }

        MenuManager.Instance.currentStageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        firstStageAudio.Stop();
        MenuManager.Instance.currentStageText.gameObject.SetActive(false);
        MenuManager.Instance.currentPlayerText.gameObject.SetActive(false);
        UpdateGameState(GameState.LoadEnemies);
    }

    /// <summary>
    /// Spawns a player instance.
    /// </summary>
    private void SpawnPlayer() {
        Vector3 spawnPos = new Vector3(-1.1f, 0f, -4f);

        if (!player.activeSelf) {
            Instantiate(playerAgent, spawnPos, Quaternion.Euler(-90, 90, 0));
        } else {
            Instantiate(player, spawnPos, Quaternion.Euler(-90, 90, 0));
        }
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
            CheckScore();
            enemyAttackState = EnemyAttackState.Done;
        }

        while (enemyAttackState != EnemyAttackState.Done) {
            // Check to see if the player has gained an extra live. 
            CheckScore();

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

            // Timeout before letting another alien attack.
            yield return new WaitForSeconds(2f);

            if (aliens.Count == 0) {
                enemyAttackState = EnemyAttackState.Done;
            } else {
                ResetAliens(currAliensAttacking);
            }
        }

        if (training) {
            PlayerAgent playerAgent = player.GetComponentInChildren<PlayerAgent>();
            playerAgent.AddReward(1f);
            playerAgent.EndEpisode();
            yield break;
        }

        if (aliens.Count == 0) {
            currentStage[currentPlayer - 1]++;
            UpdateGameState(GameState.DisplayStageText);
            yield break;
        }
    }

    /// <summary>
    /// Checks score to see if the current player has gained an extra life.
    /// </summary>
    private void CheckScore() {
        if (scores[currentPlayer - 1] == 20000
            || scores[currentPlayer - 1] % 60000 == 0) {
            lives[currentPlayer - 1]++;
            UpdateLivesTextField();
        }
    }

    /// <summary>
    /// Clears the entire alien grid from the grid object.
    /// </summary>
    public void ClearAliens() {
        spawnerController.ClearGrid();
    }

    private void ResetAliens(List<GameObject> currAliensAttacking) {
        int numOfAliensAttacking = currAliensAttacking.Count;

        while (currAliensAttacking.Count != 0) {
            if (currAliensAttacking[numOfAliensAttacking - 1] != null) {
                AlienController alienController = currAliensAttacking[--numOfAliensAttacking].GetComponent<AlienController>();
                alienController.ResetToPosition = true;
                currAliensAttacking.RemoveAt(numOfAliensAttacking);
            }
            // If training is enabled, break this loop. 
            else if (training) {
                break;
            }
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
    PlayerDeath,
    SwitchPlayer,
    GameOver,
    ResetEpisode
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