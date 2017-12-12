using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board {

    protected int player = 1;

    public Board()
    {
        player = 1-player;
    }

    public virtual Move[] GetMoves()
    {
        return new Move[0];
    }

    public virtual Board MakeMove(Move m)
    {
        return new Board();
    }

    public virtual bool IsGameOver()
    {
        return true;
    }


    public virtual float Evaluate(int player)
    {
        return Mathf.NegativeInfinity;
    }
    public virtual float Evaluate()
    {
        return Mathf.NegativeInfinity;
    }
}
