/**********************************************************
 * Player Agent
 * 
 * Summary: The learning agent for the player.
 * 
 * Author: Kurt Campbell
 * Created: 16 April 2023
 * 
 * Copyright Cedarville University, Kurt Campbell, Jackson Isenhower,
 * Donald Osborn.
 * All rights reserved.
 *********************************************************/

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
    private GameObject parent;
    private PlayerController playerController;
    private BehaviorParameters behaviorParameters;

    public override void Initialize() {
        parent = transform.parent.gameObject;
        playerController = parent.GetComponent<PlayerController>();
        behaviorParameters = GetComponent<BehaviorParameters>();
    }

    public override void OnEpisodeBegin() {
        parent.transform.position = new Vector3(-1.1f, 0f, -4f);
        
        if (GameManager.Instance.checkGridEmpty() || GameManager.Instance.PlayerDead) {
            GameManager.Instance.PlayerDead = false;
            GameManager.Instance.UpdateGameState(GameState.DisplayStageText);
            GameManager.Instance.UpdateGameState(GameState.LoadEnemies);
        }
    }

    // Testing purposes for random input instead of neural network input.
    public override void Heuristic(in ActionBuffers actionsOut) {
        Random random = new Random();
        int horizontalInput = random.Next(-12, 13);
        int shoot = random.Next(0, 2);

        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> actions = actionsOut.DiscreteActions;

        continuousActions[0] = horizontalInput;
        actions[1] = shoot;
    }

    public override void OnActionReceived(ActionBuffers actions) {
        float horizontalInput = actions.ContinuousActions[0]; // <= 12 ? actions.DiscreteActions[0] : -12;
        bool shoot = actions.DiscreteActions[1] == 1 ? true : false;

        Debug.Log("Player Shoot Input: " + shoot);

        // Controls the player by the given actions.
        playerController.HorizontalInput = horizontalInput;

        if (shoot) {
            playerController.OnFire();
        }
    }
}
