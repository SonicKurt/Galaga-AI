/**********************************************************
 * Alien Agent
 * 
 * Summary: The learning agent for an alien.
 * 
 * Author: Kurt Campbell
 * Date: 14 April 2023
 * 
 * Copyright Cedarville University, Kurt Campbell, Jackson Isenhower,
 * Donald Osborn.
 * All rights reserved.
 *********************************************************/

using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;
using Random = System.Random;

public class AlienAgent : Agent
{
    private AlienController alienController;
    private BehaviorParameters behaviorParameters;

    [SerializeField]
    private NNModel averageModel;

    [SerializeField]
    private NNModel advancedModel;

    public override void Initialize()
    {
        GameObject parent = transform.parent.gameObject;
        alienController = parent.GetComponent<AlienController>();
        behaviorParameters = GetComponent<BehaviorParameters>();
    }

    // Testing purposes for random input to move the aliens horizontally.
    public override void Heuristic(in ActionBuffers actionsOut) {
        Random random = new Random();
        int horizontalInput = random.Next(-6, 7);
        int shoot = random.Next(0, 2);

        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> actions = actionsOut.DiscreteActions;
        continuousActions[0] = horizontalInput;
        actions[1] = shoot;
    }
    
    public override void OnActionReceived(ActionBuffers actions) {
        bool attack = alienController.Attack;
        
        if (attack) {
            float horizontalInput = actions.ContinuousActions[0];
            bool shoot = actions.DiscreteActions[1] == 1 ? true : false;
            
            alienController.HorizontalInput = horizontalInput;

            if (shoot) {
                alienController.ShootBullet();
            }
        }
    }

    /// <summary>
    /// Changes the Neural Network Model for the Alien Agent.
    /// </summary>
    /// <param name="modelType">The model you want to switch to.</param>
    public void ChangeModel(AgentType modelType) {
        if (modelType == AgentType.Average) {
            Debug.Log("Switching to Average Model.");
            SetModel("Alien", averageModel);
        } else {
            Debug.Log("Switching to Advanced Model.");
            SetModel("Alien", advancedModel);
        }

        behaviorParameters.BehaviorType = BehaviorType.InferenceOnly;
    }
}

public enum AgentType {
    Beginner,
    Average,
    Advanced
};
