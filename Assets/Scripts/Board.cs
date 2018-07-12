using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board {

    protected FigureColor player;

    public Board()
    {
        player = FigureColor.WHITE;
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

    public virtual float Evaluate()
    {
        return Mathf.NegativeInfinity;
    }
    public virtual FigureColor GetCurrentPlayer()
    {
        return player;
    }

}
