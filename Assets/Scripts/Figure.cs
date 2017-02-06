using UnityEngine;
using System.Collections;

public class Figure : MonoBehaviour {

    public Field field; // ссылка на поле которое находится под фигурой

    public FigureColor color;  // цвет фигуры
    public FigureColor oppossiteColor; // противположный цвет
    public bool tobit; // является ли фигура дамкой


	// Use this for initialization
	void Start () {
        tobit = false;
	
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
        Clear();
        Destroy(gameObject);
    }

    void OnMouseDown()
    {
        if (GameController.ActiveFigure == null) 
        {
            field.CheckNeghbors();
            GameController.ActiveFigure = gameObject;
        }
        else if(GameController.ActiveFigure == gameObject)
        {
            GameController.Clear();
        }
        else
        {
            GameController.Clear();           
            field.CheckNeghbors();
            GameController.ActiveFigure = gameObject;
        }
               
    }
}
