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

    public void Move(MoveTobit move, Vector3 pos, ref Figure[,] board)
    {
        board[move.y, move.x] = this;
        transform.position = new Vector3(pos.x, pos.y, pos.z - 1);
        board[y, x] = null;
        x = move.x;
        y = move.y;
        if (move.haveKill)
        {
            Destroy(board[move.delY, move.delX].gameObject);
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
        List<Move> moves = new List<Move>();
        int[] moveX = new int[] { -1, 1 };
        int[] moveY = new int[] { -1, 1 };
        if (color == FigureColor.BLACK)
            moveY = new int[] { 1, -1 };
        foreach (int mX in moveX)
        {
            if (y == 0 || y == board.GetLength(0))
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
                m.x = nextX;
                m.y = y;
            }
            else
            {
                int hopX = nextX + mX;
                if (!IsMoveInBounds(hopX, y, ref board))
                    continue;
                if (board[y, hopX] != null)
                    continue;
                m.x = hopX;
                m.y = y;
                m.haveKill = true;
                m.delX = nextX;
                m.delY = y;
            }
            moves.Add(m);
        }

        bool first = true;

        foreach (int mY in moveY)
        {

            first = false;

            if (x == 0 || x == board.GetLength(1))
                continue;
            Debug.Log("Y check! 1");
            int nextY = y + mY;
            if (!IsMoveInBounds(x, nextY, ref board))
                continue;
            Debug.Log("Y check! 2");
            Figure f = board[nextY, x];
            if (f != null && f.color == color)
                continue;
            Debug.Log("Y check! 3");
            MoveTobit m = new MoveTobit();
            m.figure = this;
            if (!first)
            {
                Debug.Log("Y check!");
                if (f == null)
                {                    
                    m.x = x;
                    m.y = nextY;
                }
                else
                {
                    int hopY = nextY + mY;
                    if (!IsMoveInBounds(x, hopY, ref board))
                        continue;
                    if (board[hopY, x] != null)
                        continue;
                    m.x = x;
                    m.y = hopY;
                    m.haveKill = true;
                    m.delX = x;
                    m.delY = nextY;
                }                
            }
            else
            {
                if (f == null)
                    continue;
                int hopY = nextY + mY;
                if (!IsMoveInBounds(x, hopY, ref board))
                    continue;
                if (board[hopY, x] != null)
                    continue;
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

        var moveXY = new[] { new { moveX = 1, moveY = 0 },
            new { moveX = -1, moveY = 0 },
            new { moveX = 0, moveY = 1 },
            new { moveX = 0, moveY = -1}
        };
        foreach (var mXY in moveXY)
        {
            int nextX = x + mXY.moveX;
            int nextY = x + mXY.moveY;

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
                    int hopX = nextX + mXY.moveX;
                    int hopY = nextY + mXY.moveY;
                    if (!IsMoveInBounds(hopX, hopY, ref board))
                        break;
                    m.haveKill = true;
                    m.x = hopX;
                    m.y = hopY;
                    m.delX = nextX;
                    m.delY = nextY;
                }
                moves.Add(m);
                nextX += mXY.moveX;
                nextY += mXY.moveY;
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
