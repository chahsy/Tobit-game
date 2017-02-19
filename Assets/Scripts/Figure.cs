using UnityEngine;
using System.Collections;
using System;

public class Figure : MonoBehaviour {

    public Field Field
    { 
        get
        {
            return _field;
        }
        set
        {
            if (value != null && !Tobit)
            {
                CanToTransformTobit(value);
            }
            _field = value;
            
        }
    } // ссылка на поле под фигурой
    private Collider2D currentColl; // ссылка на коллайдер
    public FigureColor color;  // цвет фигуры
    public FigureColor oppossiteColor; // противположный цвет
    [SerializeField]
    private Field _field; // ссылка на поле которое находится под фигурой
    [SerializeField]
    private Sprite _whiteTobit;
    [SerializeField]
    private Sprite _blackTobit;
    public bool Tobit { get; set; } // является ли фигура дамкой

    
	// Use this for initialization
	void Start () {
        Tobit = false;
        currentColl = gameObject.GetComponent<Collider2D>();
        currentColl.enabled = false;
        EventManager.Instance.AddListener(EVENT_TYPE.SWITCH, OnEvent);
    }
		
	
    // Очищаем ссылки из поля на фигуру и убираем ссылку на самое поле
    public void Clear()
    {
        Field.Clear();
        Field = null;
    }

    // Уничтожаем фигуру при "битье"
    public void DestroyFigure()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.SWITCH, OnEvent);
        DecrementFigures(color);
        Clear();
        Destroy(gameObject);
    }

    void OnMouseDown()
    {       
        if (Managers.GameManager.ActiveFigure == null) 
        {
            Managers.GameManager.ActiveFigure = gameObject;
            Field.CheckNeghbors();            
        }
        else if(Managers.GameManager.ActiveFigure == gameObject)
        {
            Managers.GameManager.Clear();
        }
        else
        {
            Managers.GameManager.Clear();
            Managers.GameManager.ActiveFigure = gameObject;
            Field.CheckNeghbors();
            
        }
               
    }


    public void OnEvent(EVENT_TYPE Event_Type, GameObject sender, object Param = null)
    {

        switch (Event_Type)
        {
            case EVENT_TYPE.SWITCH:                
                SwitchCollider((FigureColor)Param);
                break;

        }
    }

    // функция включает или выключает коллайдер
    private void SwitchCollider(FigureColor param)
    {        
        if (color == param)
        {
            currentColl.enabled = true;
        }
        else
        {
            currentColl.enabled = false;
        }
    }

    //Функция обновляет количество фигур в менеджере
    private void DecrementFigures(FigureColor color)
    {
        if(color == FigureColor.Black)
        {
            Managers.GameManager.BlackFigures--;
        }
        if(color == FigureColor.White)
        {
            Managers.GameManager.WhiteFigures--;
        }
    }

    //Проверяем может ли фигура стать дамкой (по правилам игры дамка называется Тобит).
    private void CanToTransformTobit(Field value)
    {
        if (color == FigureColor.White)
        {
            string[] namesOfFields = { "34", "35", "36", "37", "38" };
            foreach (string nameOfField in namesOfFields)
            {
                if (nameOfField == value.name)
                {
                    Tobit = true;
                    gameObject.GetComponent<SpriteRenderer>().sprite = _whiteTobit;
                }
            }
        }
        if (color == FigureColor.Black)
        {
            string[] namesOfFields = { "1", "2", "3", "4", "5" };
            foreach (string nameOfField in namesOfFields)
            {
                if (nameOfField == value.name)
                {
                    Tobit = true;
                    gameObject.GetComponent<SpriteRenderer>().sprite = _blackTobit;
                }
            }
        }
    }
}
