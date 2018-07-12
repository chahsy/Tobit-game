using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController Instance
    {
        get
        {
            return _instance;
        }
    }    
    private static GameController _instance;

    public FigureColor playerColor;
    public FigureColor AIColor;
    public BoardTobit CurrentBoard { get; set; }
    public Field[,] deskView;
    public GameObject figurePrefab;
    public bool isGameOver;
    public FigureColor activePlayer;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
        CurrentBoard = new BoardTobit();
        deskView = new Field[CurrentBoard.rowsOfBoard, CurrentBoard.columnsOfBoard];
        isGameOver = CurrentBoard.IsGameOver();
        activePlayer = CurrentBoard.CurrentPlayer;

    }

    void Start()
    {
        playerColor = FigureColor.WHITE;
        AIColor = FigureColor.BLACK;
        ViewFigure figure = figurePrefab.GetComponent<ViewFigure>();
        if (figure == null)
        {
            Debug.LogError("No figure component detected");
            return;
        }
        int r;
        int c;
        int figuresLeft = CurrentBoard.numFigures;
        for (r = 0; r < CurrentBoard.rowsOfBoard; r++)
        {
            if (figuresLeft == 0)
                break;
            int init = 0;
            int max = CurrentBoard.columnsOfBoard;
            if (r == 0)
            {
                init = 1;
                max = CurrentBoard.columnsOfBoard - 1;
            }
            for (c = init; c < max; c++)
            {
                if (figuresLeft == 0)
                    break;
                PlaceFigures(c, r, FigureColor.WHITE);
                figuresLeft--;
            }
        }

        figuresLeft = CurrentBoard.numFigures;
        for (r = CurrentBoard.rowsOfBoard - 1; r >= 0; r--)
        {
            if (figuresLeft == 0)
                break;
            int init = 0;
            int max = CurrentBoard.columnsOfBoard;
            if (r == CurrentBoard.rowsOfBoard - 1)
            {
                init = 1;
                max = CurrentBoard.columnsOfBoard - 1;
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

    public void ChangePlayer()
    {
        activePlayer = (activePlayer == FigureColor.BLACK)? FigureColor.WHITE:FigureColor.BLACK;
        CurrentBoard.CurrentPlayer = activePlayer;
        if(activePlayer == AIColor)
        {
            MoveAI();
        }
    }

    public void HighLightMoves(Move[] playerMoves)
    {
        foreach(MoveTobit m in playerMoves)
        {
            Field illuminatedField = deskView[m.row, m.col];
            illuminatedField.MoveOn = m;            
        }
    }
    
    private void PlaceFigures(int x, int y, FigureColor color)
    {

        Vector3 pos = deskView[y, x].transform.position;
        GameObject gameObj = Instantiate(figurePrefab);
        gameObj.transform.position = new Vector3(pos.x, pos.y, pos.z - 1);
        ViewFigure gameFigure = gameObj.GetComponent<ViewFigure>();
        Figure figure = new Figure(x, y, color);
        gameFigure.SetStartProperties(figure);
        CurrentBoard.board[y, x] = figure;
    }

    private void MoveAI()
    {
        Move m = null;
        float t =  BoardTobitAI.MiniMaxAlgorithm(CurrentBoard, CurrentBoard.GetCurrentPlayer(), 5, 0, ref m);//5 это макc глубина
        MoveTobit bestMove = (MoveTobit)m;
        Debug.Log("best score " + t);        
        Figure figure = bestMove.figure;
        figure.MoveAI(bestMove, ref CurrentBoard.board);
        ChangePlayer();
    }
    
}
