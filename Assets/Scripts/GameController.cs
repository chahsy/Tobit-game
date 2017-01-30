using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    // Активная фигура для перемещения
    public static GameObject ActiveFigure { get; set; }
    // Фигуры для поедения - Надо доработать? Dictionary?    
    public static List<Figure> DestroyFigure;
    // Есть ход для "битья" фигуры
    public static bool HaveKill;

    void Start()
    {
        DestroyFigure = new List<Figure>();
        ActiveFigure = null;
        HaveKill = false;
        
    }

    // Очистка переменных и подсцветки
    public static void Clear()
    {
        if(ActiveFigure != null)
            ActiveFigure = null;
        if (DestroyFigure.Count > 0)
            DestroyFigure.Clear();
        if (HaveKill)
            HaveKill = false;
        EventManager.Instance.PostNotification(EVENT_TYPE.DEFAULT);
    }

    
}
