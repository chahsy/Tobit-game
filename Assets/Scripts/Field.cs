using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Field : MonoBehaviour {

    #region Переменные
    //TODO: Убрать ссылку на поле, оставить только цвет от фигуры, сделать метод для присвоения.
    public Figure figureOnField;    // фигура над полем 
    public FigureColor colorOfField;
    public FigureColor oppositeColorOfField;
    public bool tobitOnField;
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



    // Use this for initialization
    void Start () {

        coll2D = gameObject.GetComponent<Collider2D>();        
        Neighbors = new Dictionary<DirectionEnum, Field>();
        TobitNeighbors = new Dictionary<DirectionEnum, List<Field>>();
        FigureColors(figureOnField);
        

        EventManager.Instance.AddListener(EVENT_TYPE.DEFAULT, OnEvent);

    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
    // Отклик на нажатие мыши
    void OnMouseDown()
    {
       
        if(!Managers.GameManager.HaveKill)
        {
            //TODO: Добавить звуковой эффект
            TranslateFigure();
            Managers.GameManager.Clear();
            Managers.GameManager.ChangeActivePlayer();
        }
        else
        {
            //TODO: Добавить звуковой эффект
            TranslateFigure();
            Managers.GameManager.HaveKill = false;
            CheckNextKill();
            if (!Managers.GameManager.HaveKill)
            {
                Managers.GameManager.Clear();
                Managers.GameManager.ChangeActivePlayer();
            }
        }

    }

    //Проверка на следующее поедание фигуры
    private void CheckNextKill()
    {
        if (Managers.GameManager.DestroyFigures.Count > 0)
        {
            Dictionary<DirectionEnum, Field> bufferDictionary = new Dictionary<DirectionEnum, Field>(Managers.GameManager.DestroyFigures);
            Managers.GameManager.DestroyFigures.Clear();
            foreach (var field in bufferDictionary)
            {
                if (TobitNeighbors.ContainsKey(field.Key))
                {
                    if (TobitNeighbors[field.Key].Contains(field.Value))
                    {
                        field.Value.figureOnField.DestroyFigure();
                        EventManager.Instance.PostNotification(EVENT_TYPE.DEFAULT);
                        CheckNeghbors(field.Key);
                    }
                }
            }
        }    
    }

    //Перемещние фигуры на новое поле
    private void TranslateFigure()
    {
        Figure current = Managers.GameManager.ActiveFigure.GetComponent<Figure>();
        current.Clear();
        current.Field = gameObject.GetComponent<Field>();
        current.Field.FigureColors(current);
        current.transform.position = gameObject.transform.position;
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
        if(!kill)
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

    // Работа с свойствами, в зависимости есть фигура над полем или нет
    public void FigureColors(Figure figu)
    {
        if (figu != null)
        {
            figureOnField = figu;
            colorOfField = figu.color;
            oppositeColorOfField = figu.oppossiteColor;
            tobitOnField = figu.Tobit;
        }
        else
        {
            figureOnField = figu;
            colorOfField = FigureColor.Empty;
            oppositeColorOfField = FigureColor.Empty;
            tobitOnField = false;
        }
    }

    //проверка на пустое поле
    public bool CheckEmpty() {
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
    public bool CheckKill(FigureColor color)
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

    // Проверка соседей по очереди
    public void CheckNeghbors(DirectionEnum exceptionDirection = DirectionEnum.Nothing)
    {
        if (!tobitOnField)
        {
            foreach (var neighbor in Neighbors)
            {
                if (neighbor.Key != exceptionDirection)
                {
                    neighbor.Value.CheckFigure(colorOfField, neighbor.Key);
                }
            }
        }
        if (tobitOnField)
        {
            Managers.UtilityManager.ProcessCourseTobit(this);
        }
    }
   

    // Сравнение соседей
    public void CheckFigure(FigureColor color,DirectionEnum direction)
    {
        if (CheckEmpty() && !Managers.GameManager.HaveKill)
        {
            if (Managers.GameManager.ActiveFigure.GetComponent<Figure>().Tobit)
            {
                Accsess(false);
            }
            else
            {
                if (color == FigureColor.White && direction != DirectionEnum.Down) // ходим только вперед, вправо и лево за белых
                    Accsess(false);
                if (color == FigureColor.Black && direction != DirectionEnum.Up) // ходим только вперед, вправо и лево за черных
                    Accsess(false);
            }
        }
        if(CheckKill(color))
        {
            CheckForKill(direction);
        }
    }

    // Проверка поля перед тем как бить
    public void CheckForKill(DirectionEnum dir)
    {
        if (Neighbors.ContainsKey(dir))
        {
            if (Neighbors[dir].CheckEmpty())
            {
                if (!Managers.GameManager.HaveKill)
                {
                    EventManager.Instance.PostNotification(EVENT_TYPE.DEFAULT);
                    Managers.GameManager.HaveKill = true;
                    Neighbors[dir].Accsess(true);                   
                }
                else
                {
                    Neighbors[dir].Accsess(true);                    
                }
                Managers.GameManager.AddFigure(Managers.UtilityManager.ConvertDirection(dir), gameObject.GetComponent<Field>());
            }
        }
    }

    public void OnEvent(EVENT_TYPE Event_Type, GameObject sender, object Param = null)
    {

        switch (Event_Type)
        {
            case EVENT_TYPE.DEFAULT:
                Default();
                break;

        }
    }

    // Возвращает все поля в которых есть продолжение поедания фигуры соперника
    public List<Field> FindAllNextFields(DirectionEnum direction)
    {
        List<Field> subFindList = new List<Field>();

        foreach (var neighbor in TobitNeighbors[direction])
        {
            if (neighbor.CheckEmpty())
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
    private bool IspectionForKillInCollection(DirectionEnum first)
    {
        bool trigger = false;

        Field subField = TobitNeighbors[first].Find(Managers.UtilityManager.FindFirstNonEmptyInCollection);        
        if (subField != null && subField.colorOfField != Managers.GameManager.ActiveFigure.GetComponent<Figure>().color)
        {
            if (subField.Neighbors.ContainsKey(first))
            {
                if (subField.Neighbors[first].CheckEmpty())
                {
                    trigger = true;
                }
            }
        }      


        return trigger;
        
    }
}
