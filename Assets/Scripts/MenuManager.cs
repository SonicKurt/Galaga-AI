/**********************************************************
 * Menu Manager
 * 
 * Summary: Manages the game's flow.
 * 
 * Author: Kurt Campbell
 * Date: 23 March 2023
 * 
 * Copyright Cedarville University, Kurt Campbell, Jackson Isenhower,
 * Donald Osborn.
 * All rights reserved.
 *********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    // Main User Interface.
    public Canvas canvas;

    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI currentPlayerText;
    public TextMeshProUGUI currentStageText;

    // Player input actions for the main menu.
    private PlayerInput playerInput;

    // Main Menu Panel from the Main User Interface.
    private GameObject mainMenuPanel;

    private void Awake() {
        Instance = this;
        playerInput = GetComponent<PlayerInput>();
        // Make the menu manager be a part of every scene throughout the game.
        DontDestroyOnLoad(Instance);
        // Make the main user interface be a part of every scene throughout the game.
        DontDestroyOnLoad(canvas);
    }

    // Start is called before the first frame update
    void Start() {
       mainMenuPanel = GameObject.FindGameObjectWithTag("MainMenu");
    }

    // Update is called once per frame
    void Update() {
        
    }

    /// <summary>
    /// Player input action to start a one player game.
    /// </summary>
    void OnOnePlayerGame() {
        StartOnePlayerGame();
    }

    /// <summary>
    /// Player input action to start a two player game.
    /// </summary>
    void OnTwoPlayerGame() {
        StartTwoPlayerGame();
    }

    /// <summary>
    /// Start a one player game.
    /// </summary>
    public void StartOnePlayerGame() {
        GameManager.Instance.PlayerCount = 1;
        GameManager.Instance.UpdateGameState(GameState.DisplayStageText);
        DisableMainMenu();
        SceneManager.LoadScene(1);
        playerInput.enabled = false;
    }

    /// <summary>
    /// Start a two player game.
    /// </summary>
    public void StartTwoPlayerGame() {
        GameManager.Instance.PlayerCount = 2;
        GameManager.Instance.UpdateGameState(GameState.DisplayStageText);
        DisableMainMenu();
        SceneManager.LoadScene(1);
        playerInput.enabled = false;
    }

    /// <summary>
    /// Update a player's score text field.
    /// </summary>
    /// <param name="score">The current score for the player.</param>
    /// <param name="player">The current player to update.</param>
    public void UpdateScoreTextFields(int score, int player) {
        if (player == 1) {
            player1ScoreText.SetText(score.ToString());
        } else {
            player2ScoreText.SetText(score.ToString());
        }
    }

    /// <summary>
    /// Update the high score text field.
    /// </summary>
    /// <param name="score"></param>
    public void UpdateHighScoreTextField(int score) {
        highScoreText.SetText(score.ToString());
    }

    /// <summary>
    /// Update the current player text field.
    /// </summary>
    /// <param name="player"></param>
    public void UpdateCurrentPlayerTextField(int player) {
        currentPlayerText.SetText("Player " + player);
    }

    /// <summary>
    /// Update the current stage text field.
    /// </summary>
    /// <param name="stage"></param>
    public void UpdateCurrentStageTextField(int stage) {
        currentStageText.SetText("Stage " + stage);
    }

    /// <summary>
    /// Disable the main menu within the main user interface.
    /// </summary>
    private void DisableMainMenu() {
        mainMenuPanel.SetActive(false);
    }

    /// <summary>
    /// Enable the main menu within the main user interface.
    /// </summary>
    public void EnableMainMenu() {
        mainMenuPanel.SetActive(true);
    }
}
