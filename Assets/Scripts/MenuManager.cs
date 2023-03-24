using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public Canvas canvas;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI currentPlayerText;
    public TextMeshProUGUI currentStageText;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(Instance);
        DontDestroyOnLoad(canvas);
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }

    public void StartOnePlayerGame() {
        GameManager.Instance.PlayerCount = 1;
        GameManager.Instance.UpdateGameState(GameState.DisplayStageText);
        DisableMainMenu();
        SceneManager.LoadScene(1);
    }

    public void StartTwoPlayerGame() {
        GameManager.Instance.PlayerCount = 2;
        GameManager.Instance.UpdateGameState(GameState.DisplayStageText);
        DisableMainMenu();
        SceneManager.LoadScene(1);
    }

    public void UpdateScoreTextFields(int score, int player) {
        if (player == 1) {
            player1ScoreText.SetText(score.ToString());
        } else {
            player2ScoreText.SetText(score.ToString());
        }
    }

    public void UpdateHighScoreTextField(int score) {
        highScoreText.SetText(score.ToString());
    }

    public void UpdateCurrentPlayerTextField(int player) {
        currentPlayerText.SetText("Player " + player);
    }

    public void UpdateCurrentStageTextField(int stage) {
        currentStageText.SetText("Stage " + stage);
    }

    private void DisableMainMenu() {
        foreach (Transform uiChild in canvas.GetComponentInChildren<Transform>()) {
            if (uiChild.gameObject.tag != "Score") {
                uiChild.gameObject.SetActive(false);
            }
        }
    }
}
