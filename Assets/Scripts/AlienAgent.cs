using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
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
        // NOTE: This code snippet would be useful for the player, not the aliens.
        /*
        BehaviorParameters behaviorParameters = GetComponent<BehaviorParameters>();
        if (behaviorParameters.BehaviorType == BehaviorType.Default) {
            GameManager.Instance.PlayerCount = 1;
            GameManager.Instance.Training = true;
            GameManager.Instance.UpdateGameState(GameState.LoadEnemies);
        }
        */



    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        
    }

    public override void OnActionReceived(ActionBuffers actions) {
        bool attack = alienController.Attack;
        
        if (attack) {

        }

    }
}
