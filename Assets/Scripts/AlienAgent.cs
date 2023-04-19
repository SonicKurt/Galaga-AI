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

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;
using Random = System.Random;

public class AlienAgent : Agent
{
    private AlienController alienController;
    private BehaviorParameters behaviorParameters;

    public override void Initialize()
    {
        GameObject parent = transform.parent.gameObject;
        alienController = parent.GetComponent<AlienController>();
        behaviorParameters = GetComponent<BehaviorParameters>();
    }

    // Testing purposes for random input to move the aliens horizontally.
    public override void Heuristic(in ActionBuffers actionsOut) {
        Random random = new Random();
        int horizontalInput = random.Next(-12, 13);
        int shoot = random.Next(0, 2);

        ActionSegment<int> actions = actionsOut.DiscreteActions;
        actions[0] = horizontalInput;
        actions[1] = shoot;
    }
    
    public override void OnActionReceived(ActionBuffers actions) {
        bool attack = alienController.Attack;
        
        if (attack) {
            float horizontalInput = actions.DiscreteActions[0] <= 12 ? actions.DiscreteActions[0] : -12;
            bool shoot = actions.DiscreteActions[1] == 1 ? true : false;
            
            alienController.HorizontalInput = horizontalInput;

            if (shoot) {
                alienController.ShootBullet();
            }
        }
    }
}
