using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BoardTobitAI
{


    public static float MiniMaxAlgorithm(
        Board board, 
        FigureColor curPlayer, 
        int maxDepth, 
        int currentDepth, 
        ref Move bestMove)
    {

        if (board.IsGameOver() || currentDepth == maxDepth)
        {
            return board.Evaluate();
        }

        bestMove = null;
        float bestScore = Mathf.NegativeInfinity;
        if (curPlayer != board.GetCurrentPlayer())
            bestScore = Mathf.Infinity;
        Move[] moves = board.GetMoves();


        foreach (Move m in moves)
        {
            Board b = board.MakeMove(m);
            Move currentMove = null;
            float recursedScore = -MiniMaxAlgorithm(b, curPlayer, maxDepth, currentDepth + 1, ref currentMove);
            if (currentDepth == 0)
            {                
                Debug.Log("Score:  " + recursedScore);
            }
            if (curPlayer == board.GetCurrentPlayer())
            {
                if (recursedScore > bestScore)
                {
                    bestScore = recursedScore;
                    bestMove = m;
                }
            }
            else
            {
                if (recursedScore < bestScore)
                {
                    bestScore = recursedScore;
                    bestMove = m;
                }
            }
        }
        return bestScore;
    }

}
