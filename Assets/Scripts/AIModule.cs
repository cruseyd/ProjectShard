using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIStrategy
{
    RANDOM,
    SIMULATE
}

public class AIModule : MonoBehaviour
{

    public List<ICommand> FindMoves()
    {
        Actor actor = GetComponent<Actor>();
        List<ICommand> moves = new List<ICommand>();
        foreach(Card card in actor.active)
        {
            moves.AddRange(card.FindMoves());
        }
        foreach (Card card in actor.hand)
        {
            moves.AddRange(card.FindMoves());
        }
        return moves;
    }

    public ICommand ChooseMove(AIStrategy strategy = AIStrategy.RANDOM)
    {
        List<ICommand> moves = FindMoves();
        if (moves.Count > 0)
        {
            if (strategy == AIStrategy.RANDOM)
            {
                return moves[Random.Range(0, moves.Count)];
            } else if (strategy == AIStrategy.SIMULATE)
            {
                GameState gameState = new GameState(Enemy.instance);
                float baseValue = gameState.Evaluate();
                float baseValueSelf = gameState.EvaluateSelf();
                float baseValueEnemy = gameState.EvaluateOpponent();
                float moveValue = baseValue - 5;
                ICommand chosen = null;
                foreach (ICommand move in moves)
                {
                    move.Execute(gameState);
                    float value = gameState.Evaluate();
                    //Debug.Log("Evaluating move: " + move.name + " | result: " + value +
                    //    " | base: " + baseValue + "(" + baseValueSelf + ", " + baseValueEnemy + ")");
                    if (value > moveValue)
                    {
                        if (value < baseValue) //if this move is worse than doing nothing...
                        {
                            float selfValue = gameState.EvaluateSelf();
                            float enemyValue = gameState.EvaluateOpponent();
                            if ((enemyValue - baseValueEnemy) > (selfValue - baseValueSelf))
                            {
                                // ... and it *only* helps the opponent, then don't do this. 
                                continue;
                            }
                        }
                        moveValue = value;
                        chosen = move;
                    }
                    move.Undo(gameState);
                }
                return chosen;
            }
        }
        return null;
    }
}
