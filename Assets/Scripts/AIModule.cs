using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIModule : MonoBehaviour
{

    public List<ICommand> FindMoves()
    {
        Actor actor = GetComponent<Actor>();
        Debug.Log("Finding moves for actor " + actor.name);
        List<ICommand> moves = new List<ICommand>();
        foreach(Card card in actor.active)
        {
            moves.AddRange(card.FindMoves());
        }
        foreach (Card card in actor.hand)
        {
            moves.AddRange(card.FindMoves());
        }
        Debug.Log("AI module found " + moves.Count + " moves");
        return moves;
    }

    public ICommand ChooseMove()
    {
        List<ICommand> moves = FindMoves();
        if (moves.Count > 0)
        {
            return moves[Random.Range(0, moves.Count)];
        }
        return null;
    }
}
