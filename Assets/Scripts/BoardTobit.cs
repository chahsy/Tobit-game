using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardTobit : Board {

    public FigureColor CurrentPlayer
    {
        get
        {
            if(player == 1)
            {
                return FigureColor.BLACK;
            }
            else
            {
                return FigureColor.WHITE;
            }
        }
        set
        {
            if(value == FigureColor.WHITE)
            {
                player = 0;
            }
            else
            {
                player = 1;
            }
        }
    }
    public int rowsOfBoard = 6;
    public int columnsOfBoard = 7;
    public Figure[,] board;
    public int numFigures = 12;

    public BoardTobit() : base()
    {
        
        board = new Figure[rowsOfBoard, columnsOfBoard];

    }

    public BoardTobit(BoardTobit boardMover)
    {
        player = 1 - boardMover.player;
        board = new Figure[rowsOfBoard, columnsOfBoard];
        Array.Copy(boardMover.board, 0, board, 0, boardMover.board.Length);
    }
    public override Board MakeMove(Move m)
    {
        BoardTobit movedBoard = new BoardTobit(this);
        MoveTobit currentMove = m as MoveTobit;
        Figure fig = new Figure(currentMove.figure.x,currentMove.figure.y,currentMove.figure.color,currentMove.figure.type);
        movedBoard.board[currentMove.y, currentMove.x] = fig;
        movedBoard.board[fig.y, fig.x] = null;

        fig.x = currentMove.x;
        fig.y = currentMove.y;
        if (currentMove.haveKill)
        {
            movedBoard.board[currentMove.delY, currentMove.delX] = null;
        }
        if (currentMove.figure.type == FigureType.SUPER)
            return movedBoard;
        int rows = movedBoard.board.GetLength(0) - 1;
        if (currentMove.figure.color == FigureColor.WHITE && currentMove.figure.y == rows)
        {
            currentMove.figure.type = FigureType.SUPER;
        }
        if (currentMove.figure.color == FigureColor.BLACK && currentMove.figure.y == 0)
        {
            currentMove.figure.type = FigureType.SUPER;
        }
        return movedBoard;
    }
    public override bool IsGameOver()
    {        
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Figure f = board[i, j];
                if(f != null && f.color == CurrentPlayer)
                {
                    return false;
                }
               
            }
        }
        return true;
    }

    public override float Evaluate()
    {
        FigureColor color = CurrentPlayer;       
        return Evaluate(color);
    }

    //TODO: Необходимо сделать более продуманный подсчет ходов
    private float Evaluate(FigureColor color) 
    {
        float eval = 1f;
        float pointSimple = 1f;
        float pointSuccess = 10f;
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        int i;
        int j;
        for (i = 0; i < rows; i++)
        {
            for (j = 0; j < cols; j++)
            {
                Figure f = board[i, j];
                if (f == null)
                    continue;
                if (f.color != color)
                    continue;
                Move[] moves = f.GetMoves(ref board);
                foreach (Move mv in moves)
                {
                    MoveTobit m = (MoveTobit)mv;
                    if (m.haveKill)
                    {
                        eval += pointSuccess;                        
                    }
                    else
                    {
                        eval += pointSimple;
                    }
                    if (IsImrpovedMove(color, m))
                        eval += pointSuccess;
                }
            }
        }
        return eval;
    }

    private bool IsImrpovedMove(FigureColor color, MoveTobit m)
    {
        int rows = board.GetLength(0) - 1;
        if (color == FigureColor.WHITE && m.y == rows)
        {
            return true;
        }
        if (color == FigureColor.BLACK && m.y == 0)
        {
            return true;

        }
        return false;
    }    

    public override Move[] GetMoves()
    {
        List<Move> moves = new List<Move>();
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        FigureColor color = CurrentPlayer;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Figure f = board[i, j];
                if (f == null || f.color != color)
                    continue;
                moves.AddRange(f.GetMoves(ref board));
            }
        }
        if(moves.Exists(m => (m as MoveTobit).haveKill))
            moves = moves.FindAll(m => (m as MoveTobit).haveKill);
        return moves.ToArray();
    }
    
        
}
