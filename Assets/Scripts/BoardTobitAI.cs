using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BoardTobitAI {
    
    
    public static float Negamax(
            Board board,
            int maxDepth,
            int currentDepth,
            ref Move bestMove)
    {
        
        if (board.IsGameOver() || currentDepth == maxDepth)
            return board.Evaluate();
        bestMove = null;
        float bestScore = Mathf.NegativeInfinity;       
        foreach (Move m in board.GetMoves())
        {
            Board b = board.MakeMove(m);
            float recursedScore;
            Move currentMove = null;
            recursedScore = Negamax(b, maxDepth, currentDepth + 1, ref currentMove);
            float currentScore = recursedScore; // убрал "-" с recursedScore
            if (currentScore > bestScore)
            {
                bestScore = currentScore;
                bestMove = m;
            }
        }
        return bestScore;
    }
}
