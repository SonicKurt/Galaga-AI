using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;

public class AlienAgent : Agent
{
    private AlienController alienController;
    private BehaviorParameters behaviorParameters;

    public override void Initialize()
    {
        alienController = GetComponent<AlienController>();
        behaviorParameters = GetComponent<BehaviorParameters>();
    }

    /*
    public override void OnEpisodeBegin()
    {
        // NOTE: This code snippet would be useful for the player, not the aliens.
        
        BehaviorParameters behaviorParameters = GetComponent<BehaviorParameters>();
        if (behaviorParameters.BehaviorType == BehaviorType.Default) {
            GameManager.Instance.PlayerCount = 1;
            GameManager.Instance.Training = true;
            GameManager.Instance.UpdateGameState(GameState.LoadEnemies);
        }
        



    }
    */

    public override void Heuristic(in ActionBuffers actionsOut) {
        int horizontalInput = Mathf.RoundToInt(alienController.HorizontalInput);

        ActionSegment<int> actions = actionsOut.DiscreteActions;
        actions[0] = horizontalInput;
    }
    
    public override void OnActionReceived(ActionBuffers actions) {
        bool attack = alienController.Attack;
        
        if (attack) {
            float horizontalInput = actions.DiscreteActions[0] <= 12 ? actions.DiscreteActions[0] : -12;

            if (behaviorParameters.BehaviorType != BehaviorType.HeuristicOnly) {
                alienController.HorizontalInput = horizontalInput;
            }
        }
    }
}
