using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour, IGameManager {
    
    private bool changeActivePlayer;
    

    //TODO: Доработать, вызов метода о прекращении игры если кто то достиг 0.
    //TODO: Добавить UI, статистика количество фиугр на поле
    public int WhiteFigures {
        get
        {
          return _whiteFigures;
        }
        set
        {
          _whiteFigures = value;
          Debug.Log("Осталось белых фигур: " + _whiteFigures);
        }
    }

    public int BlackFigures
    {
        get
        {
            return _blackFigures;
        }
        set
        {
            _blackFigures = value;
            Debug.Log("Осталось черных фигур: " + _blackFigures);
        }
    }

    private int _whiteFigures;
    private int _blackFigures;
    private GameObject _activeFigure;

    // Активная фигура для перемещения
    public GameObject ActiveFigure
    {
        get
        {
            return _activeFigure;
        }
        set
        {            
            _activeFigure = value;
        }
    }
    //Статус контроллера
    public ManagerStatus status { get; private set; }

    // Фигуры для поедения  
    public Dictionary<DirectionEnum, Field> DestroyFigures;
    // Есть ход для "битья" фигуры
    public bool HaveKill { get; set; }


    // Очистка переменных и подсцветки
    public void Clear()
    {
        if (Managers.GameManager.ActiveFigure != null)
            Managers.GameManager.ActiveFigure = null;
        if (Managers.GameManager.DestroyFigures.Count > 0)
            Managers.GameManager.DestroyFigures.Clear();
        if (Managers.GameManager.HaveKill)
            Managers.GameManager.HaveKill = false;        
        EventManager.Instance.PostNotification(EVENT_TYPE.DEFAULT);
    }

    // Добавление фигур для "битья", проверка перед добавлением.
    public void AddFigure(DirectionEnum dir, Field fil)
    {
        if (Managers.GameManager.DestroyFigures.ContainsKey(dir))
        {
            Managers.GameManager.DestroyFigures[dir] = fil;
        }
        else
        {
            Managers.GameManager.DestroyFigures[dir] = fil;
        }
    }

    // Запуск контроллера, начальные настройки
    public void Startup()
    {
        Debug.Log("Game manager starting...");
        DestroyFigures = new Dictionary<DirectionEnum, Field>();
        ActiveFigure = null;
        HaveKill = false;
        WhiteFigures = 0;
        BlackFigures = 0;
        changeActivePlayer = true;  
        StartCoroutine(FindFigures());
        status = ManagerStatus.Started;
    }

    //Поиск фигур на поле, для отчета
    private IEnumerator FindFigures()
    {
        yield return new WaitForSeconds(1);


        WhiteFigures = FiguresNumbers("White");
        Debug.Log("Белых фигур на поле: " + WhiteFigures);

        yield return null;

        BlackFigures = FiguresNumbers("Black");
        Debug.Log("Черных фигур на поле: " + BlackFigures);

        yield return null;

        ChangeFigureColliders(changeActivePlayer);
    }

    // Функция возварщает количество объектов с определенным ТЭГОМ
    private int FiguresNumbers(string tag)
    {
        GameObject[] bufferWhiteGameObjects = GameObject.FindGameObjectsWithTag(tag);
        return bufferWhiteGameObjects.Length;
    }

    //Функция смены активного игрока (белые или черные фигуры) 
    public void ChangeActivePlayer() {

        if (changeActivePlayer)
        {
            changeActivePlayer = false;
            //TODO: Добавить в настройках данную функцибю Camera.main.transform.Rotate(0, 0, 180);                    
            ChangeFigureColliders(changeActivePlayer);
            
        }
        else
        {
            changeActivePlayer = true;
            //TODO: Добавить в настройках данную функцибю Camera.main.transform.Rotate(0, 0, 180); 
            ChangeFigureColliders(changeActivePlayer);
            
        }
        
    }
    //вызывает события включения отключения коллайдера у фигур
    private void ChangeFigureColliders(bool activePlayer)
    {
        if (activePlayer)
        {
            EventManager.Instance.PostNotification(EVENT_TYPE.SWITCH, Param: FigureColor.White);
        }
        else
        {
            EventManager.Instance.PostNotification(EVENT_TYPE.SWITCH, Param: FigureColor.Black);
        }
    }
}
