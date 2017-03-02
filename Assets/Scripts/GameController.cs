using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour, IGameManager {
    
    private bool changeActivePlayer;
    [SerializeField]
    private Canvas EndGameCanvas;
    
    //TODO: Добавить UI, статистика количество фиугр на поле
    public int WhiteFigures {
        get
        {
          return _whiteFigures;
        }
        set
        {
          _whiteFigures = value;
          if(_whiteFigures == 0)
            {
                WinEvent(FigureColor.Black);
            }
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
            if (_blackFigures == 0)
            {
                WinEvent(FigureColor.White);
            }
        }
    }

    private void WinEvent(FigureColor figure)
    {        
        if(figure == FigureColor.White)
        {
            EndGameCanvas.gameObject.SetActive(true);
            EndGameCanvas.transform.FindChild("WinTextWhite").gameObject.SetActive(true);
        }
        if (figure == FigureColor.Black)
        {
            EndGameCanvas.gameObject.SetActive(true);
            EndGameCanvas.transform.FindChild("WinTextBlack").gameObject.SetActive(true);
        }
    }

    private int _whiteFigures;
    private int _blackFigures;
    private Figure _activeFigure;

    // Активная фигура для перемещения
    public Figure ActiveFigure
    {
        get
        {
            return _activeFigure;
        }
        set
        {                              
            _activeFigure = value;
            if(_activeFigure != null)
            {
                _activeFigure.AllocateFigureEnable();
            }
        }
    }
    //Статус контроллера
    public ManagerStatus status { get; private set; }

    // Фигура для поедения  
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
        changeActivePlayer = true;  
        StartCoroutine(FindFigures());
        status = ManagerStatus.Started;
    }

    //Поиск фигур на поле, для отчета
    private IEnumerator FindFigures()
    {
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("White").Length == 12);
        WhiteFigures = GameObject.FindGameObjectsWithTag("White").Length;
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Black").Length == 12);
        BlackFigures = GameObject.FindGameObjectsWithTag("Black").Length;

        yield return null;

        ChangeFigureColliders(changeActivePlayer);
    }
    

    //Функция смены активного игрока (белые или черные фигуры) 
    public void ChangeActivePlayer() {

        if (changeActivePlayer)
        {
            changeActivePlayer = false;                           
            ChangeFigureColliders(changeActivePlayer);
            
        }
        else
        {
            changeActivePlayer = true;           
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
