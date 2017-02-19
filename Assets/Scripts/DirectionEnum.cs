using UnityEngine;
using System.Collections;


public interface IGameManager
{
    ManagerStatus status { get; }

    void Startup();
}


// Перечисление. В какие направления можно ходить с этого поля.
public enum DirectionEnum { 
    Up,
    Down,
    Right,
    Left,
    Nothing               
}


// Состояние поля. Пустое поле или какого цвета находится шашка на поле
public enum FigureColor
{
    Empty,
    Black,
    White
}

public enum ManagerStatus
{
    Shutdown,
    Initializing,
    Started
}
