using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardTobit : Board {

    public static BoardTobit Instance
    {
        get
        {
            return _instance;
        }
    }
    public int rowsOfBoard = 6;
    public int columnsOfBoard = 7;
    public int numFigures = 12;
    public GameObject prefab;
    public Field[,] fieldsOnBoard;

    public Figure[,] board;

    private static BoardTobit _instance;
    

    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
        player = 0;
        board = new Figure[rowsOfBoard, columnsOfBoard];
        fieldsOnBoard = new Field[rowsOfBoard, columnsOfBoard];
    }

    void Start()
    {        
        Figure figure = prefab.GetComponent<Figure>();
        if (figure == null)
        {
            Debug.LogError("No figure component detected");
            return;
        }
        int r;
        int c;
        int figuresLeft = numFigures;
        for (r = 0; r < rowsOfBoard; r++)
        {
            if (figuresLeft == 0)
                break;
            int init = 0;
            int max = columnsOfBoard;
            if (r == 0)
            {
                init = 1;
                max = columnsOfBoard - 1;
            }
            for (c = init; c < max; c++)
            {
                if (figuresLeft == 0)
                    break;
                PlaceFigures(c, r, FigureColor.WHITE);
                figuresLeft--;
            }
        }

        figuresLeft = numFigures;
        for (r = rowsOfBoard - 1; r >= 0; r--)
        {
            if (figuresLeft == 0)
                break;
            int init = 0;
            int max = columnsOfBoard;
            if (r == rowsOfBoard-1)
            {
                init = 1;
                max = columnsOfBoard - 1;
            }
            for (c = init; c < max; c++)
            {
                if (figuresLeft == 0)
                    break;
                PlaceFigures(c, r, FigureColor.BLACK);
                figuresLeft--;
            }
        }
    }

    private void PlaceFigures(int x, int y, FigureColor color)
    {
        
        Vector3 pos = fieldsOnBoard[y, x].transform.position;
        GameObject go = Instantiate(prefab);
        go.transform.position = new Vector3(pos.x,pos.y,pos.z-1);
        Figure p = go.GetComponent<Figure>();
        p.SetStartProperties(x, y, color);
        board[y, x] = p;
    }

    public override float Evaluate()
    {
        FigureColor color = FigureColor.WHITE;
        if (player == 1)
            color = FigureColor.BLACK;
        return Evaluate(color);
    }

    public override float Evaluate(int player)
    {
        FigureColor color = FigureColor.WHITE;
        if (player == 1)
            color = FigureColor.BLACK;
        return Evaluate(color);
    }

    private float Evaluate(FigureColor color)
    {
        float eval = 1f;
        float pointSimple = 1f;
        float pointSuccess = 5f;
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        int i;
        int j;
        for (i = 0; i < rows; i++)
        {
            for (j = 0; j < cols; j++)
            {
                Figure p = board[i, j];
                if (p == null)
                    continue;
                if (p.color != color)
                    continue;
                Move[] moves = p.GetMoves(ref board);
                foreach (Move mv in moves)
                {
                    MoveTobit m = (MoveTobit)mv;
                    if (m.haveKill)
                        eval += pointSuccess;
                    else
                        eval += pointSimple;
                }
            }
        }
        return eval;
    }

    public override Move[] GetMoves()
    {
        List<Move> moves = new List<Move>();
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        int i;
        int j;
        for (i = 0; i < rows; i++)
        {
            for (j = 0; i < cols; j++)
            {
                Figure p = board[i, j];
                if (p == null)
                    continue;
                moves.AddRange(p.GetMoves(ref board));
            }
        }
        return moves.ToArray();
    }
}
