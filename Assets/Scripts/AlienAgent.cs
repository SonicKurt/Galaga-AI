using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class AlienAgent : Agent
{
    private AlienController alienController;

    public override void Initialize()
    {
        alienController = GetComponent<AlienController>();
    }

    public override void OnEpisodeBegin()
    {
        GameManager.Instance.Training = true;
        GameManager.Instance.UpdateGameState(GameState.LoadEnemies);

    }

    
}
