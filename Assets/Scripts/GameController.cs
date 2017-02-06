using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    //TODO: Контроль хода, отключение и включение коллайдеров, для белых или черных фигур

    //TODO: Контроль выйгрыша, если количество фигур белых или черных = 0, то выйграли черные или белые.

    //TODO: Синглтон паттерн, контроллер должен быть один, оформить диспетчер контроллеров (GameController EventManager), использовать готовый фреймворк


    

    // Активная фигура для перемещения
    public static GameObject ActiveFigure { get; set; }
    // Фигуры для поедения  
    public static Dictionary<DirectionEnum,Field> DestroyFigures;
    // Есть ход для "битья" фигуры
    public static bool HaveKill;

    void Start()
    {
        DestroyFigures = new Dictionary<DirectionEnum, Field>();
        ActiveFigure = null;
        HaveKill = false;
        
    }

    // Очистка переменных и подсцветки
    public static void Clear()
    {
        if(ActiveFigure != null)
            ActiveFigure = null;
        if (DestroyFigures.Count > 0)
            DestroyFigures.Clear();
        if (HaveKill)
            HaveKill = false;
        EventManager.Instance.PostNotification(EVENT_TYPE.DEFAULT);
    }

    // Добавление фигур для "битья", проверка перед добавлением.
    public static void AddFigure(DirectionEnum dir, Field fil)
    {
        if(DestroyFigures.ContainsKey(dir))
        {
            DestroyFigures[dir] = fil;
        }
        DestroyFigures.Add(dir, fil);
    }

    
}
