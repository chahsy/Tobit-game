﻿using UnityEngine;
using System.Collections;



/// <summary>
/// Перечисление. В какие направления можно ходить с этого поля.
/// </summary>
public enum DirectionEnum { 
    Up,
    Down,
    Right,
    Left,
    Nothing               
}

/// <summary>
/// Состояние поля. Пустое поле или какого цвета находится шашка на поле
/// </summary>
public enum FigureColor
{
    Empty,
    Black,
    White
}
