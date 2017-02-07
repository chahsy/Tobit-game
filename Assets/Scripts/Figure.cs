using UnityEngine;
using System.Collections;
using System;

public class Figure : MonoBehaviour {

    public Field field; // ссылка на поле которое находится под фигурой
    private Collider2D currentColl; 
    public FigureColor color;  // цвет фигуры
    public FigureColor oppossiteColor; // противположный цвет
    public bool tobit; // является ли фигура дамкой


	// Use this for initialization
	void Start () {
        tobit = false;
        currentColl = gameObject.GetComponent<Collider2D>();
        currentColl.enabled = false;
        EventManager.Instance.AddListener(EVENT_TYPE.SWITCH, OnEvent);
    }
		
	
    // Очищаем ссылки из поля на фигуру и убираем ссылку на самое поле
    public void Clear()
    {
        field.Clear();
        field = null;
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
            field.CheckNeghbors();
            Managers.GameManager.ActiveFigure = gameObject;
        }
        else if(Managers.GameManager.ActiveFigure == gameObject)
        {
            Managers.GameManager.Clear();
        }
        else
        {
            Managers.GameManager.Clear();           
            field.CheckNeghbors();
            Managers.GameManager.ActiveFigure = gameObject;
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
}
