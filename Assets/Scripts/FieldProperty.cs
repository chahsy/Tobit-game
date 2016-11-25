using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FieldProperty : MonoBehaviour {
    /// <summary>
    /// Направления с этого поля
    /// </summary>
    public DirectionEnum Direction { get { return direction; } }
    /// <summary>
    /// Ссылка на соседние поля
    /// </summary>
    public Dictionary<DirectionEnum,GameObject> Neighbors { get { return neighbors; } }

    /// <summary>
    /// Возможные направления с этого поля (Вверх, вниз и т.д.)
    /// </summary>
    [SerializeField]
    private DirectionEnum direction;
    /// <summary>
    /// Словарь ссылок на соседние поля с ключом по направлению (сверху, снизу и т.д.)
    /// </summary>
    private Dictionary<DirectionEnum, GameObject> neighbors;
    #region Данные для Raycast
    //Дистанция для Raycast 
    public float distance;
    //Слой для Raycast
    public LayerMask layer;
    /// <summary>
    /// Переменные Vector2 origin для Raycast, исключаем прохождения луча через поле на котором находится фигура.
    /// </summary>
    private Vector2 left_ray;
    private Vector2 right_ray;
    private Vector2 up_ray;
    private Vector2 down_ray;
    #endregion

    /// <summary>
    /// Свойства для чтения текущего состояния поля
    /// </summary>
    public CheckOnField OnFieldCheck { get { return onField; } } 
    /// <summary>
    /// Закрытая перменная состояния поля
    /// </summary>
    private CheckOnField onField;

    void Awake() {
        neighbors = new Dictionary<DirectionEnum, GameObject>();
        onField = CheckOnField.Empty;
        left_ray = (Vector2)gameObject.transform.position + new Vector2(-0.4f, 0);
        right_ray = (Vector2)gameObject.transform.position + new Vector2(0.4f, 0);
        up_ray = (Vector2)gameObject.transform.position + new Vector2(0, 0.4f);
        down_ray = (Vector2)gameObject.transform.position + new Vector2(0, -0.4f);
    }

	void Update()
    {
        if (Input.GetKeyDown("space"))
        {            
            InspectionNearFields(Direction);           
        }
    }


    /// <summary>
    /// Задает какого цвета фигура сейчас над полем находится.
    /// </summary>
    /// <param name="value">Значение цвета шашки</param>
    public void SetOnField(CheckOnField value)
    {
        onField = value;
    }

    /// <summary>
    /// Проверяет поля при помощие Raycast и записывает данные в словарь Neighbors
    /// </summary>
    /// <param name="dir">Передается направления проверки</param>
    public void InspectionNearFields(DirectionEnum dir)
    {
        RaycastHit2D hit;
        switch (dir)
        {
            case DirectionEnum.Anywhere:
                goto case DirectionEnum.Down;
            case DirectionEnum.Down:               
                hit = Physics2D.Raycast(down_ray, Vector2.down, distance, layer);
                if(hit.collider !=null)
                    neighbors.Add(DirectionEnum.Down, hit.collider.gameObject);
                if (dir == DirectionEnum.Anywhere)
                    goto case DirectionEnum.Left;                
                break;
            case DirectionEnum.Left:
                hit = Physics2D.Raycast(left_ray, Vector2.left, distance, layer);
                if (hit.collider != null)
                    neighbors.Add(DirectionEnum.Left, hit.collider.gameObject);
                if (dir == DirectionEnum.Anywhere)
                    goto case DirectionEnum.Right;
                break;
            case DirectionEnum.Right:
                hit = Physics2D.Raycast(right_ray, Vector2.right, distance, layer);
                if (hit.collider != null)
                    neighbors.Add(DirectionEnum.Right, hit.collider.gameObject);
                if (dir == DirectionEnum.Anywhere)
                    goto case DirectionEnum.Up;
                break;
            case DirectionEnum.Up:
                hit = Physics2D.Raycast(up_ray, Vector2.up, distance, layer);
                if (hit.collider != null)
                    neighbors.Add(DirectionEnum.Up, hit.collider.gameObject);
                break;
        }
        
                       
    }

}
