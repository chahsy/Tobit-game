using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Field : MonoBehaviour {

    #region Переменные    
    public Figure figureOnField;    // фигура над полем 
    public FigureColor colorOfField;
    public FigureColor oppositeColorOfField;
    
    #region Neighbors
    public Dictionary<DirectionEnum, Field> Neighbors;  // ссылки на соседние поля
    public Dictionary<DirectionEnum, List<Field>> TobitNeighbors;  
    #endregion

    public Sprite red; // Подсцветка полей во время хода
    public Sprite green;

    public bool FieldCollEnable // Включает или отключает collider поля
    {
        set
        {
            coll2D.enabled = value;
        }
    } 

    private Collider2D coll2D;


    #endregion

    public void OnEvent(EVENT_TYPE Event_Type, GameObject sender, object Param = null)
    {

        switch (Event_Type)
        {
            case EVENT_TYPE.DEFAULT:
                Default();
                break;

        }
    }

    // Сброс свойств в стандарт, объект не доступен для манипуляций
    public void Default()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = null;
        FieldCollEnable = false;
    }
    // Доступ к полю для хода
    public void Accsess(bool kill)
    {        
        if (!kill)
            gameObject.GetComponent<SpriteRenderer>().sprite = green;
        if (kill)
            gameObject.GetComponent<SpriteRenderer>().sprite = red;
        FieldCollEnable = true;        
    }
    // Опустошение поля, значение Empty
    public void Clear()
    {
        figureOnField = null;
        FigureColors(figureOnField);
    }
    //проверка на пустое поле
    public bool MayBeEmpty()
    {
        if (colorOfField == FigureColor.Empty)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //можно ли съесть фигуру
    public bool MayBeOppositeColor(FigureColor color)
    {
        if (color == oppositeColorOfField)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Use this for initialization
    void Start () {

        coll2D = gameObject.GetComponent<Collider2D>();        
        Neighbors = new Dictionary<DirectionEnum, Field>();
        TobitNeighbors = new Dictionary<DirectionEnum, List<Field>>();
        FigureColors(figureOnField);
        

        EventManager.Instance.AddListener(EVENT_TYPE.DEFAULT, OnEvent);

    }

    // Работа с свойствами, в зависимости есть фигура над полем или нет
    public void FigureColors(Figure figu)
    {
        if (figu != null)
        {
            figureOnField = figu;
            colorOfField = figu.color;
            oppositeColorOfField = figu.oppossiteColor; 
        }
        else
        {
            figureOnField = figu;
            colorOfField = FigureColor.Empty;
            oppositeColorOfField = FigureColor.Empty;            
        }
    }


    // Отклик на нажатие мыши
    void OnMouseDown()
    {
       
        if(!Managers.GameManager.HaveKill)
        {
            //TODO: Добавить звуковой эффект
            Managers.UtilityManager.TranslateFigure(gameObject.GetComponent<Field>());
            Managers.GameManager.Clear();
            Managers.GameManager.ChangeActivePlayer();
        }
        else
        {
            //TODO: Добавить звуковой эффект
            Managers.UtilityManager.TranslateFigure(gameObject.GetComponent<Field>());
            Managers.GameManager.HaveKill = false;
            Managers.UtilityManager.CheckNextKill(this);
            if (!Managers.GameManager.HaveKill)
            {
                Managers.GameManager.Clear();
                Managers.GameManager.ChangeActivePlayer();
            }
        }

    }
    
    // Возвращает все поля в которых есть продолжение поедания фигуры соперника
    public List<Field> FindAllNextFields(DirectionEnum direction)
    {
        List<Field> subFindList = new List<Field>();

        foreach (var neighbor in TobitNeighbors[direction])
        {
            if (neighbor.MayBeEmpty())
            {
                if (direction == DirectionEnum.Up || direction == DirectionEnum.Down)
                {
                    if (neighbor.Neighbors.ContainsKey(DirectionEnum.Right))
                    {
                        if (neighbor.IspectionForKillInCollection(DirectionEnum.Right))
                        {
                            subFindList.Add(neighbor);
                        }
                    }
                    if (!subFindList.Contains(neighbor))
                    {
                        if (neighbor.Neighbors.ContainsKey(DirectionEnum.Left))
                        {
                            if (neighbor.IspectionForKillInCollection(DirectionEnum.Left))
                            {
                                subFindList.Add(neighbor);
                            }
                        }
                    }
                                      
                }
                if (direction == DirectionEnum.Right || direction == DirectionEnum.Left)
                {
                    if (neighbor.Neighbors.ContainsKey(DirectionEnum.Up))
                    {
                        if (neighbor.IspectionForKillInCollection(DirectionEnum.Up))
                        {
                            subFindList.Add(neighbor);
                        }
                    }
                    if (!subFindList.Contains(neighbor))
                    {
                        if (neighbor.Neighbors.ContainsKey(DirectionEnum.Down))
                        {
                            if (neighbor.IspectionForKillInCollection(DirectionEnum.Down))
                            {
                                subFindList.Add(neighbor);
                            }
                        }
                    }
                }
            }
            else
            {
                break;
            }
        }
        return subFindList;
    }

    // Возвращает true если есть возможность для поедании фигуры сопераника (дамкой) в этом направлении
    private bool IspectionForKillInCollection(DirectionEnum direct)
    {
        bool trigger = false;

        Field subField = TobitNeighbors[direct].Find(Managers.UtilityManager.FindFirstNonEmptyInCollection);        
        if (subField != null && subField.colorOfField != Managers.GameManager.ActiveFigure.GetComponent<Figure>().color)
        {
            if (subField.Neighbors.ContainsKey(direct))
            {
                if (subField.Neighbors[direct].MayBeEmpty())
                {
                    trigger = true;
                }
            }
        }      


        return trigger;
        
    }

}
