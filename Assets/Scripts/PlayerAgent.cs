using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class PlayerAgent : Agent
{
    private PlayerController playerController;
    private BehaviorParameters behaviorParameters;

    public override void Initialize() {
        playerController = GetComponent<PlayerController>();
        behaviorParameters = GetComponent<BehaviorParameters>();
    }

    public override void OnEpisodeBegin() {
        // NOTE: This code snippet would be useful for the player, not the aliens.

        GameManager.Instance.UpdateGameState(GameState.DisplayStageText);

        /*
        if (behaviorParameters.BehaviorType == BehaviorType.Default) {
           
        }
        */
    }

    // Testing purposes for human input instead of neural network input.
    public override void Heuristic(in ActionBuffers actionsOut) {
        Random random = new Random();
        int horizontalInput = random.Next(-12, 13);
        int shoot = random.Next(0, 2);

        ActionSegment<int> actions = actionsOut.DiscreteActions;
        actions[0] = horizontalInput;
        actions[1] = shoot;
    }

    public override void OnActionReceived(ActionBuffers actions) {
        float horizontalInput = actions.DiscreteActions[0] <= 12 ? actions.DiscreteActions[0] : -12;
        bool shoot = actions.DiscreteActions[1] == 1 ? true : false;

        playerController.HorizontalInput = horizontalInput;

        if (shoot) {
            playerController.OnFire();
        }
    }
}
