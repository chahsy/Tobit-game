using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardTobit : Board {
    [HideInInspector]
    public int rowsOfBoard = 6;
    [HideInInspector]
    public int columnsOfBoard = 7;
    [HideInInspector]
    public int numFigures = 12;
    public Figure[,] board;

    #region Оценочные значения
    float pointSide = 1f;//очки за ход в бок
    float pointForward = 2f;// очки за продвижение вперед
    float pointKill = 10f;// очки за поедание фигуры
    float pointType = 15f;// очки за переход в дамку
    #endregion

    public FigureColor CurrentPlayer {
        get
        {
            return player;
        }
        set
        {
            player = value;
        }
    }
    public static FigureColor IsOppositeColor(FigureColor curColor)
    {
       
            return curColor == FigureColor.BLACK ? FigureColor.WHITE : FigureColor.BLACK;
        
    }


    public BoardTobit() : base()
    {

        board = new Figure[rowsOfBoard, columnsOfBoard];

    }
    public BoardTobit(BoardTobit boardMover) 
        : this()
    {
        CurrentPlayer = (boardMover.CurrentPlayer == FigureColor.WHITE) ? FigureColor.BLACK : FigureColor.WHITE;
        CopyBoard(boardMover, board);
    }

    private void CopyBoard(BoardTobit boardMover, Figure[,] newboard)
    {
        Figure[,] oldBoard = boardMover.board;
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (oldBoard[i, j] != null)
                {
                    newboard[i, j] = new Figure(oldBoard[i, j]);
                }
            }
        }
    }


    //Делаем ход "фигурой"
    public override Board MakeMove(Move m)
    {
        BoardTobit movedBoard = new BoardTobit(this);
        MoveTobit currentMove = (MoveTobit)m;
        Figure fig = new Figure(currentMove.figure);
        fig.Move(currentMove,ref movedBoard.board);
        if (currentMove.haveKill)
        {
            Move[] nextKillMoves = fig.GetMoves(ref movedBoard.board);
            if(Array.Exists(nextKillMoves, x => ((MoveTobit)x).haveKill))
            {
                Move nextMove = Array.Find(nextKillMoves, x => ((MoveTobit)x).haveKill);
                movedBoard = (BoardTobit)movedBoard.MakeMove(nextMove);
            }
        }
        
        return movedBoard;
    }

    // Проверка на окончание игры. Игра окончена, когда у одного из игроков на поле не осталось фигур
    public override bool IsGameOver()
    {
        
        List<Figure> currentFigures = GetListFigures(board);// находим все фигуры на поле                                                            
        // Находим есть ли у игроков фигуры. 
        //Если у одного из игроков нет фигур на поле то возвращаем true, в ином случае вернется false. (используем исключающее ИЛИ)        
        return currentFigures.Exists(x => x.Color == FigureColor.WHITE) ^ currentFigures.Exists(x => x.Color == FigureColor.BLACK); 
    }

    
    public override float Evaluate()
    {
        #region Проверка на выйгрыш
        
        List<Figure> currentFigures = GetListFigures(board);
        //если фигур у игрока не осталось и он проиграл
        if (!currentFigures.Exists(x => x.Color == CurrentPlayer))
            return Mathf.NegativeInfinity;
        //если фигур у противника не осталось то текущий игрок выйграл
        if (!currentFigures.Exists(x => x.Color == IsOppositeColor(CurrentPlayer)))
            return Mathf.Infinity;
        #endregion
        //Оцениваем игровое поле относительной текущего игрока
        return Evaluate(CurrentPlayer);
    }

    public override FigureColor GetCurrentPlayer()
    {
        return CurrentPlayer;
    }

    private float Evaluate(FigureColor color) 
    {
        float eval = 0f;       

        List<Figure> oppositeFigures; //массив фигур противника
        List<Figure> currentFigures; //массив фигур текущего игрока
        float oppositePlayerPoints = 0f;//оценка позиции на поле активного игрока
        float currentPlayerPoints = 0f;//оценка позиции противника
        
        // получаем все фигуры на поле
        List<Figure> activeFigures = GetListFigures(board);

        // оцениваем количество фигур игроков
        oppositeFigures = activeFigures.FindAll(o => o.Color == IsOppositeColor(CurrentPlayer));
        currentFigures = activeFigures.FindAll(c => c.Color == color);
        float oppositePointsOfType = oppositeFigures.FindAll(s => s.Type == FigureType.SUPER).Count * pointType + oppositeFigures.Count;
        float currentPointsOfType = currentFigures.FindAll(s => s.Type == FigureType.SUPER).Count * pointType + currentFigures.Count;
        
        // оценка позиции на поле и потенциальных ходов
        oppositePlayerPoints = EvaluatePosition(oppositeFigures);
        currentPlayerPoints = EvaluatePosition(currentFigures);

        eval += (currentPlayerPoints + currentPointsOfType - oppositePlayerPoints - oppositePointsOfType);
      
        return eval;
    }

    //Оцениваем позицию фигур игрока
    private float EvaluatePosition(List<Figure> activefigures)
    {
        float points = 0f;
        
        foreach (Figure f in activefigures)
        {
            //Находим все возможные ходы текущей фигуры
            Move[] moves = f.GetMoves(ref board);
            foreach (MoveTobit mv in moves)
            {
                points += IsImrpovedType(f.Color, mv);// если фигура станет "дамкой" добавляем очки, если нет то прибавляем 0
                if (mv.haveKill)
                {
                    points += DoKill(mv, this);// Если ход с поеданием противника, подсчитываем все поедание и прибавляем
                }
                else // Обычный ход
                {
                    if (f.row - mv.row != 0)
                    {
                        points += pointForward; //Ходим вперед по полю
                    }
                    else
                        points += pointSide; //Ходим вбок
                }
            }
        }

        return points;
    }

    //Проверка на дамку
    private float IsImrpovedType(FigureColor color, MoveTobit m)
    {
        int rows = board.GetLength(0) - 1;
        if (color == FigureColor.WHITE && m.row == rows)
        {
            return pointType;
        }
        if (color == FigureColor.BLACK && m.row == 0)
        {
            return pointType;

        }
        return 0f;
    }
    
    // Оценка за поедание фигур
    private float DoKill(MoveTobit move, BoardTobit currentDesk)
    {
        float points = pointKill; // Начальное значение оценки
      
        FigureColor oppositePlayer = IsOppositeColor(move.figure.Color); //текущий цвет противника
        List<Figure> oldFigures = GetListFigures(currentDesk.board).FindAll(x => x.Color == oppositePlayer); //находим все фигуры противника до хода
        BoardTobit b = (BoardTobit)currentDesk.MakeMove(move);        //Делаем ход с поеданием фигуры противника  
        List<Figure> newFigures = GetListFigures(b.board).FindAll(x => x.Color == oppositePlayer);
        if (newFigures.Count == 0)// провеяем остались ли фигуры у противника
            return Mathf.Infinity;
        points += pointKill * (oldFigures.Count - newFigures.Count);
        return points;
    }

    //Возвращаем массив всех фигур на поле
    private List<Figure> GetListFigures(Figure[,] board)
    {
        List<Figure> activeFiguresOnBoard = new List<Figure>();
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);       
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Figure f = board[i, j];
                if (f == null)
                    continue;
                else
                    activeFiguresOnBoard.Add(f);
                
            }
        }

        return activeFiguresOnBoard;
    }

    // Возвращаем все возможные ходы текущего игрока
    public override Move[] GetMoves()
    {
        List<Move> moves = new List<Move>();
        List<Figure> figures = GetListFigures(board);
        figures = figures.FindAll(x => x.Color == CurrentPlayer);
        foreach(Figure f in figures)
        {
            moves.AddRange(f.GetMoves(ref board));
        }
        if (moves.Exists(x => ((MoveTobit)x).haveKill))
        {
            moves = moves.FindAll(x => ((MoveTobit)x).haveKill);
        }
        return moves.ToArray();
    }

    


   
    

}
