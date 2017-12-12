using System;
using System.Collections.Generic;
using UnityEngine;


public class Figure {

    public delegate void Moved(MoveTobit move);
    public delegate void Destroed();
    public delegate void BecameSuper();

    public int x;
    public int y;
    public FigureColor color;
    public FigureType type;    
    public event Moved MovedEvent;
    public event Destroed DestroyEvent;
    public event BecameSuper BecameSuperEvent;

    
    public Figure()
    {
        x = 0;
        y = 0;
        color = FigureColor.BLACK;
        type = FigureType.NORMAL;
    }

    public Figure(int x, int y, FigureColor color, FigureType type = FigureType.NORMAL)
    {
        this.x = x;
        this.y = y;
        this.color = color;
        this.type = type;
    }
        

    public void Move(MoveTobit move, ref Figure[,] board)
    {
        MovedEvent(move);
        board[move.y, move.x] = this;
        board[y, x] = null;
        x = move.x;
        y = move.y;
        if (move.haveKill)
        {
            if (board[move.delY, move.delX] != null)
            {
                board[move.delY, move.delX].DestroyEvent();
            }
            board[move.delY, move.delX] = null;
        }
        if (type == FigureType.SUPER)
            return;
        int rows = board.GetLength(0)-1;
        if (color == FigureColor.WHITE && y == rows)
        {
            type = FigureType.SUPER;
            BecameSuperEvent();
        }
        if (color == FigureColor.BLACK && y == 0)
        {
            type = FigureType.SUPER;
            BecameSuperEvent();
        }
        
    }

    public Move[] GetMoves(ref Figure[,] board)
    {
        List<Move> moves = new List<Move>();
        if (type == FigureType.SUPER)
            moves = GetMovesSuper(ref board);
        else
            moves = GetMovesNormal(ref board);
        return moves.ToArray();
    }

    //TODO: возвращаем список ходов при поедании
    private List<Move> GetMovesNormal(ref Figure[,] board)
    {
        List<Move> moves = new List<Move>();
        bool canKill = false;
        int[] moveX = new int[] { -1, 1 };
        int[] moveY = new int[] { 1, -1 };
        if (color == FigureColor.BLACK)
            moveY = new int[] { -1, 1 };
        //Проверяем возможность ходить вправо и влево
        foreach (int mX in moveX)
        {
            if (y == 0 || y == board.GetLength(0)-1)
                continue;
            int nextX = x + mX;
            if (!IsMoveInBounds(nextX, y, ref board))
                continue;

            Figure f = board[y, nextX];
            if (f != null && f.color == color)
                continue;
            MoveTobit m = new MoveTobit();
            m.figure = this;
            if (f == null)
            {
                if (!canKill)
                {
                    m.x = nextX;
                    m.y = y;
                }
                else
                    continue;
            }
            else
            {                
                int hopX = nextX + mX;
                if (!IsMoveInBounds(hopX, y, ref board))
                    continue;
                if (board[y, hopX] != null)
                    continue;
                if (!canKill)
                {
                    moves.Clear();
                    canKill = true;
                }
                m.x = hopX;
                m.y = y;
                m.haveKill = true;                
                m.delX = nextX;
                m.delY = y;
            }
            moves.Add(m);
        }

        
        //Проверяем возможность ходить прямо или бить назад
        foreach (int mY in moveY)
        {

            if (x == 0 || x == board.GetLength(1)-1)
                continue;
            int nextY = y + mY;
            if (!IsMoveInBounds(x, nextY, ref board))
                continue;
            Figure f = board[nextY, x];
            if (f != null && f.color == color)
                continue;
            MoveTobit m = new MoveTobit();
            m.figure = this;
            if (f == null)
            {
                if (!canKill && mY == moveY[0])
                {
                    m.x = x;
                    m.y = nextY;
                }
                else
                    continue;
            }
            else
            {
                int hopY = nextY + mY;
                if (!IsMoveInBounds(x, hopY, ref board))
                    continue;
                if (board[hopY, x] != null)
                    continue;
                if (!canKill)
                {
                    moves.Clear();
                    canKill = true;
                }
                m.x = x;
                m.y = hopY;
                m.haveKill = true;
                m.delX = x;
                m.delY = nextY;
            }                
                   
            moves.Add(m);
        }
        return moves;
    }
    
    private List<Move> GetMovesSuper(ref Figure[,] board)
    {
        List<Move> moves = new List<Move>();
        bool canKill = false;
        int[] moveX = new int[] { -1, 1 };
        int[] moveY = new int[] { -1, 1 };

        foreach (var mX in moveX)
        {
            if (y == 0 || y == board.GetLength(0) - 1)
                continue;
            int nextX = x + mX;
            int nextY = y;
            MoveTobit killField = null;


            while (IsMoveInBounds(nextX, nextY, ref board))
            {
                Figure p = board[nextY, nextX];
                if (p != null && p.color == color)
                    break;
                MoveTobit m = new MoveTobit();
                m.figure = this;
                if (p == null)
                {
                    if (!canKill)
                    {
                        m.x = nextX;
                        m.y = nextY;
                        
                    }
                    else
                    {
                        if (killField != null)
                        {
                            m.x = nextX;
                            m.y = nextY;
                            m.haveKill = true;
                            m.delX = killField.delX;
                            m.delY = killField.delY;
                        }
                        else
                        {
                            nextX += mX;
                            continue;
                        }
                    }
                }
                else
                {
                    int hopX = nextX + mX;
                    int hopY = nextY;
                    if (!IsMoveInBounds(hopX, hopY, ref board))
                        break;
                    if (board[hopY, hopX] != null)
                        break;
                    if (!canKill)
                    {
                        moves.Clear();
                        canKill = true;
                    }
                    m.haveKill = true;
                    m.x = hopX;
                    m.y = hopY;
                    m.delX = nextX;
                    m.delY = nextY;
                    killField = m;
                    nextX = hopX;
                }
                moves.Add(m);
                nextX += mX;
            }
            
        }
        foreach (var mY in moveY)
        {
            if (x == 0 || x == board.GetLength(1) - 1)
                continue;
            int nextX = x;
            int nextY = y + mY;
            MoveTobit killField = null;


            while (IsMoveInBounds(nextX, nextY, ref board))
            {
                Figure p = board[nextY, nextX];
                if (p != null && p.color == color)
                    break;
                MoveTobit m = new MoveTobit();
                m.figure = this;
                if (p == null)
                {
                    if (!canKill)
                    {
                        m.x = nextX;
                        m.y = nextY;
                       
                    }
                    else
                    {
                        if (killField != null)
                        {
                            m.x = nextX;
                            m.y = nextY;
                            m.haveKill = true;
                            m.delX = killField.delX;
                            m.delY = killField.delY;
                        }
                        else
                        {
                            nextY += mY;
                            continue;
                        }
                    }
                }
                else
                {
                    int hopX = nextX;
                    int hopY = nextY + mY;
                    if (!IsMoveInBounds(hopX, hopY, ref board))
                        break;
                    if (board[hopY, hopX] != null)
                        break;
                    if (!canKill)
                    {
                        moves.Clear();
                        canKill = true;
                    }
                    m.haveKill = true;
                    m.x = hopX;
                    m.y = hopY;
                    m.delX = nextX;
                    m.delY = nextY;
                    killField = m;
                    nextY = hopY;

                }
                moves.Add(m);
                nextY += mY;
            }

        }
        return moves;
    }

    private bool IsMoveInBounds(int x, int y, ref Figure[,] board)
    {
        if (x < 0 || x > board.GetLength(1)-1 || y < 0 || y > board.GetLength(0)-1)
            return false;
        return true;
    }

}


public enum FigureColor
{
    WHITE,
    BLACK    
}

public enum FigureType
{
    NORMAL,
    SUPER
}