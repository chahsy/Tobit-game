using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FigureColor
{
    BLACK,
    WHITE
}

public enum FigureType
{
    NORMAL,
    SUPER
}

public class Figure : MonoBehaviour {

    public int x;
    public int y;
    public FigureColor color;
    public FigureType type;
    public Sprite black;
    public Sprite white;

    public void SetStartProperties(int startX, int startY, FigureColor setColor, FigureType setType = FigureType.NORMAL)
    {
        x = startX;
        y = startY;
        color = setColor;
        type = setType;
        if(color == FigureColor.WHITE)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = white;
            gameObject.name = "White";
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = black;
            gameObject.name = "Black";
        }
    }

    public void Move(MoveTobit move, ref Figure[,] board)
    {
        board[move.y, move.x] = this;
        board[y, x] = null;
        x = move.x;
        y = move.y;
        if (move.haveKill)
        {
            Destroy(board[move.delY, move.delX]);
            board[move.delY, move.delX] = null;
        }
        if (type == FigureType.SUPER)
            return;
        int rows = board.GetLength(0);
        if (color == FigureColor.WHITE && y == rows)
            type = FigureType.SUPER;
        if (color == FigureColor.BLACK && y == 0)
            type = FigureType.SUPER;
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

    private List<Move> GetMovesNormal(ref Figure[,] board)
    {
        List<Move> moves = new List<Move>(2);
        int[] moveX = new int[] { -1, 1 };
        int moveY = 1;
        if (color == FigureColor.BLACK)
            moveY = -1;
        foreach (int mX in moveX)
        {
            int nextX = x + mX;
            int nextY = y + moveY;
            if (!IsMoveInBounds(nextX, y, ref board))
                continue;

            Figure p = board[moveY, nextX];
            if (p != null && p.color == color)
                continue;

            MoveTobit m = new MoveTobit();
            m.figure = this;
            if (p == null)
            {
                m.x = nextX;
                m.y = nextY;
            }
            else
            {
                int hopX = nextX + mX;
                int hopY = nextY + moveY;
                if (!IsMoveInBounds(hopX, hopY, ref board))
                    continue;
                if (board[hopY, hopX] != null)
                    continue;
                m.y = hopX;
                m.x = hopY;
                m.haveKill = true;
                m.delX = nextX;
                m.delY = nextY;
            }
            moves.Add(m);
        }
        return moves;
    }

    private List<Move> GetMovesSuper(ref Figure[,] board)
    {
        List<Move> moves = new List<Move>();
        int[] moveX = new int[] { -1, 1 };
        int[] moveY = new int[] { -1, 1 };
        foreach (int mY in moveY)
        {
            foreach (int mX in moveX)
            {
                int nextX = x + mX;
                int nextY = y + mY;
                while (IsMoveInBounds(nextX, nextY, ref board))
                {
                    Figure p = board[nextY, nextX];
                    if (p != null && p.color == color)
                        break;
                    MoveTobit m = new MoveTobit();
                    m.figure = this;
                    if (p == null)
                    {
                        m.x = nextX;
                        m.y = nextY;
                    }
                    else
                    {
                        int hopX = nextX + mX;
                        int hopY = nextY + mY;
                        if (!IsMoveInBounds(hopX, hopY, ref board))
                            break;
                        m.haveKill = true;
                        m.x = hopX;
                        m.y = hopY;
                        m.delX = nextX;
                        m.delY = nextY;
                    }
                    moves.Add(m);
                    nextX += mX;
                    nextY += mY;
                }
            }
        }
        return moves;
    }

    private bool IsMoveInBounds(int x, int y, ref Figure[,] board)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        if (x < 0 || x >= cols || y < 0 || y >= rows)
            return false;
        return true;
    }

}
