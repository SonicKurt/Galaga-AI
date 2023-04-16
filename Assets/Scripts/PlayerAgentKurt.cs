using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;

public class PlayerAgentKurt : Agent
{
    private PlayerController playerController;
    private BehaviorParameters behaviorParameters;

    public override void Initialize() {
        playerController = GetComponent<PlayerController>();
        behaviorParameters = GetComponent<BehaviorParameters>();
    }

    public override void OnEpisodeBegin() {
        // NOTE: This code snippet would be useful for the player, not the aliens.
        
        if (behaviorParameters.BehaviorType == BehaviorType.Default) {
            GameManager.Instance.PlayerCount = 1;
            GameManager.Instance.training = true;
            GameManager.Instance.UpdateGameState(GameState.LoadEnemies);
        }
    }

    // Testing purposes for human input instead of neural network input.
    public override void Heuristic(in ActionBuffers actionsOut) {
        int horizontalInput = Mathf.RoundToInt(playerController.HorizontalInput);

        ActionSegment<int> actions = actionsOut.DiscreteActions;
        actions[0] = horizontalInput;
    }

    public override void OnActionReceived(ActionBuffers actions) {
        float horizontalInput = actions.DiscreteActions[0] <= 12 ? actions.DiscreteActions[0] : -12;

        if (behaviorParameters.BehaviorType != BehaviorType.HeuristicOnly) {
            playerController.HorizontalInput = horizontalInput;
        }
    }
}
