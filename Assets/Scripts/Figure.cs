using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Класс описывающий игровую фигуру. Содержит координаты на поле и методы для совершения хода, и получения возможных ходов этой фигурой
/// </summary>
public class Figure {

    //Делегаты используемые классом
    public delegate void Moved(MoveTobit move);
    public delegate void Destroed();
    public delegate void BecameSuper();

    //Public 
    public int col; // Координата игровом поле, соответствует координате "Х" - столбец
    public int row;// Координата игровом поле, соответствует координате "Y" - строка
    public FigureColor Color { get; set; }    
    public FigureType Type { get; set; }


    //События
    public event Moved MovedEvent; // Callback когда фигура совершила move
    public event Destroed DestroyEvent; // Callback когда фигуру "съел" противник
    public event BecameSuper BecameSuperEvent;//Callback когда фигура стала "дамкой"

    //Конструкторы
    public Figure()
    {
        col = 0;
        row = 0;
        Color = FigureColor.WHITE;
        Type = FigureType.NORMAL;
    }
    public Figure(int c, int r, FigureColor color, FigureType type = FigureType.NORMAL)
    {
        col = c;
        row = r;
        Color = color;
        Type = type;
    }
    public Figure(Figure copyFigure)
    {
        col = copyFigure.col;
        row = copyFigure.row;
        Color = copyFigure.Color;
        Type = copyFigure.Type;
    }
        
    //Методы

    public void Move(MoveTobit move, ref Figure[,] board)
    {
        board[move.row, move.col] = this;
        board[row, col] = null;
        col = move.col;
        row = move.row;
        if (move.haveKill)
        {            
            board[move.delRow, move.delCol] = null;
        }
        if (Type == FigureType.SUPER)
            return;
        int rows = board.GetLength(0)-1;
        if (Color == FigureColor.WHITE && row == rows)
        {
            Type = FigureType.SUPER;
        }
        if (Color == FigureColor.BLACK && row == 0)
        {
            Type = FigureType.SUPER;
        }

    }

    //Ход игрока
    public void MovePlayer(MoveTobit move, ref Figure[,] board)
    {
        if (move.haveKill)
        {
            board[move.delRow, move.delCol].DestroyEvent();
        }
        FigureType oldType = Type; //костыль
        Move(move,ref board);
        MovedEvent(move);       
        if(oldType != Type)
        {
            BecameSuperEvent();
        }
    }

    //Ход AI
    public void MoveAI(MoveTobit move, ref Figure[,] board)
    {
        MovePlayer(move, ref board);
        if (move.haveKill)
        {
            Move[] nextKillMoves = move.figure.GetMoves(ref board);
            nextKillMoves = Array.FindAll(nextKillMoves, m => (m as MoveTobit).haveKill);
            if(nextKillMoves != null && nextKillMoves.Length > 0)
            {
                MoveTobit nextkill = (MoveTobit)nextKillMoves[0];
                MoveAI(nextkill, ref board);
            }
        }
    }

    // Получаем все возможные ходы текущей фигурой
    public Move[] GetMoves(ref Figure[,] board)
    {
        List<Move> moves = new List<Move>();
        if (Type == FigureType.SUPER)
            moves = GetMovesSuper(ref board);
        else
            moves = GetMovesNormal(ref board);
        return moves.ToArray();
    }

    /// <summary>
    /// Возвращаем все возомжные ходы для фигуры типа Normal
    /// </summary>
    /// <param name="board">Текущее состояние игрового поле</param>
    /// <returns>Массив все доступных ходов</returns>
    private List<Move> GetMovesNormal(ref Figure[,] board)
    {
        List<Move> moves = new List<Move>();
        int[] moveX = new int[] { -1, 1 };
        int[] moveY = new int[] { 1, -1 };
        if (Color == FigureColor.BLACK)
            moveY = new int[] { -1, 1 };
        //Проверяем возможность ходить вправо и влево
        foreach (int mX in moveX)
        {
            if (row == 0 || row == board.GetLength(0)-1) // если крайние строки, то нельзя ходить вправо или влево
                break;
            int nextX = col + mX;
            if (!IsMoveInBounds(nextX, row, ref board)) // проверяем край игрового поля для хода
                continue;

            Figure f = board[row, nextX];
            if (f != null && f.Color == Color) //проверяем находится ли на клетке фигура того же цвета
                continue;
            MoveTobit m = new MoveTobit();
            m.figure = this;
            if (f == null) // добавляем координаты для обычного хода
            {
                m.col = nextX;
                m.row = row;
            }
            else //добавляем координаты для хода с поеданием фигуры противника
            {                
                int hopX = nextX + mX;
                if (!IsMoveInBounds(hopX, row, ref board))
                    continue;
                if (board[row, hopX] != null)
                    continue;       
                m.col = hopX;
                m.row = row;
                m.haveKill = true;                
                m.delCol = nextX;
                m.delRow = row;
            }
            moves.Add(m);
        }
        
        //Проверяем возможность ходить прямо или бить назад
        for(int i=0; i < moveY.Length; i++)
        {            
            if (col == 0 || col == board.GetLength(1)-1) // если крайние столбцы, то нельзя ходить вправо или влево
                break;
            int nextY = row + moveY[i];
            if (!IsMoveInBounds(col, nextY, ref board)) // проверяем край игрового поля для хода
                continue;
            Figure f = board[nextY, col];
            if (f != null && f.Color == Color) //проверяем находится ли на клетке фигура того же цвета
                continue;
            MoveTobit m = new MoveTobit();
            m.figure = this;
            if (f == null) // добавляем координаты для обычного хода
            {
                if (i == 0) // это ход вперед
                {
                    m.col = col;
                    m.row = nextY;
                }
                else                //назад ходить нельзя
                    continue;
            }
            else //добавляем координаты для хода с поеданием фигуры противника
            {
                int hopY = nextY + moveY[i];
                if (!IsMoveInBounds(col, hopY, ref board))
                    continue;
                if (board[hopY, col] != null)
                    continue;              
                m.col = col;
                m.row = hopY;
                m.haveKill = true;
                m.delCol = col;
                m.delRow = nextY;
            }           
                   
            moves.Add(m);
        }
        if(moves.Exists(x => (x as MoveTobit).haveKill))
            moves = moves.FindAll(x => (x as MoveTobit).haveKill);
        return moves;
    }
    
    /// <summary>
    /// Возвращаем все возомжные ходы для фигуры типа Super ("дамка")
    /// </summary>
    /// <param name="board">Текущее состояние игрового поле</param>
    /// <returns>Массив все доступных ходов</returns>
    private List<Move> GetMovesSuper(ref Figure[,] board)
    {
        List<Move> moves = new List<Move>();
        int[] moveX = new int[] { -1, 1 };
        int[] moveY = new int[] { -1, 1 };

        foreach (var mX in moveX)
        {
            if (row == 0 || row == board.GetLength(0) - 1) // если крайние строки, то нельзя ходить вправо или влево
                break;
            int nextX = col + mX;
            int nextY = row;
            MoveTobit killMove = new MoveTobit();//Данные поедаемой фигуры, нам нужны координаты уничтожаемой с поля фигуры
            while (IsMoveInBounds(nextX, nextY, ref board)) //Идем по направляения пока не дойдем до края игрового поля
            {
                Figure p = board[nextY, nextX];
                if (p != null && p.Color == Color) // Проверка на возомжность хода
                    break;
                MoveTobit m = new MoveTobit();
                m.figure = this;
                if (p == null) 
                {
                    m.col = nextX;
                    m.row = nextY;
                    if (killMove.haveKill) //Если имеется клетка для поедания, сохраним координаты для поедания фигуры
                    {
                        m.haveKill = killMove.haveKill;
                        m.delCol = killMove.delCol;
                        m.delRow = killMove.delRow;
                    }
                }
                else
                {
                    if (!killMove.haveKill)
                    {
                        int hopX = nextX + mX;
                        int hopY = nextY;
                        if (!IsMoveInBounds(hopX, hopY, ref board))
                            break;
                        if (board[hopY, hopX] != null)
                            break;
                        m.haveKill = true;
                        m.col = hopX;
                        m.row = hopY;
                        m.delCol = nextX;
                        m.delRow = nextY;
                        nextX = hopX;
                        killMove = m;
                    }
                    else // Если имеется клетка для поедания, выходим из цикла
                        break;
                }
                moves.Add(m);
                nextX += mX;
            }
            
        }
        foreach (var mY in moveY)
        {
            if (col == 0 || col == board.GetLength(1) - 1) // если крайние столбцы, то нельзя ходить вверж или вниз
                break;
            int nextX = col;
            int nextY = row + mY;
            MoveTobit killMove = new MoveTobit();//Данные поедаемой фигуры, нам нужны координаты уничтожаемой с поля фигуры
            while (IsMoveInBounds(nextX, nextY, ref board))
            {
                Figure p = board[nextY, nextX];
                if (p != null && p.Color == Color) // Проверка на возомжность хода
                    break;
                MoveTobit m = new MoveTobit();
                m.figure = this;
                if (p == null)
                {
                    m.col = nextX;
                    m.row = nextY;
                    if (killMove.haveKill) //Если имеется клетка для поедания, сохраним координаты для поедания фигуры
                    {
                        m.haveKill = killMove.haveKill;
                        m.delCol = killMove.delCol;
                        m.delRow = killMove.delRow;
                    }
                }
                else
                {
                    if (!killMove.haveKill)
                    {
                        int hopX = nextX;
                        int hopY = nextY + mY;
                        if (!IsMoveInBounds(hopX, hopY, ref board))
                            break;
                        if (board[hopY, hopX] != null)
                            break;
                        m.haveKill = true;
                        m.col = hopX;
                        m.row = hopY;
                        m.delCol = nextX;
                        m.delRow = nextY;
                        nextY = hopY;
                        killMove = m;
                    }
                    else // Если имеется клетка для поедания, выходим из цикла
                        break;
                }
                moves.Add(m);
                nextY += mY;
            }

        }
        if (moves.Exists(x => (x as MoveTobit).haveKill))
            moves = moves.FindAll(x => (x as MoveTobit).haveKill);
        return moves;
    }

    //Проверяем выход за границы игрового поля
    private bool IsMoveInBounds(int x, int y, ref Figure[,] board)
    {
        if (x < 0 || x > board.GetLength(1)-1 || y < 0 || y > board.GetLength(0)-1)
            return false;
        return true;
    }
   

}

/// <summary>
/// Цвета фигур
/// </summary>
public enum FigureColor
{
    WHITE,
    BLACK    
}

/// <summary>
/// Типы фигур
/// </summary>
public enum FigureType
{
    NORMAL,
    SUPER
}