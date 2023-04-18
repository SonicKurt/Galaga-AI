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
        alienController = GetComponent<AlienController>();
        behaviorParameters = GetComponent<BehaviorParameters>();
    }

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
            Debug.Log("Horizontal: " + horizontalInput);


            alienController.HorizontalInput = horizontalInput;

            if (shoot) {
                alienController.ShootBullet();
            }
        }
    }
}
