using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState state;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        UpdateGameState(GameState.LoadEnemies);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (state)
        {
            case GameState.PlayerSelect:
                break;
            case GameState.DisplayStageText:
                break;
            case GameState.LoadEnemies:
                break;
            case GameState.EnemiesFall:
                break;
            default:
                break;
        }
    }
}

public enum GameState
{
    PlayerSelect,
    DisplayStageText,
    LoadEnemies,
    EnemiesFall
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
