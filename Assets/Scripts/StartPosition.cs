using UnityEngine;
using System.Collections;

public class StartPosition : MonoBehaviour {

    /// <summary>
    /// Переменные префаба черной и белой пешки
    /// </summary>
    [SerializeField]
    private GameObject blackPrefab;
    [SerializeField]
    private GameObject whitePrefab;

    /// <summary>
    /// Переменная объекта поля, нужен для position
    /// </summary>
    private GameObject field;

	// Use this for initialization
	void Start () {

        //StartCheckPosition(1, 12, CheckOnField.White);
        //StartCheckPosition(27, 38, CheckOnField.Black); Убрать после тестов

        StartCheckPosition(8, 8, CheckOnField.White);
        //StartCheckPosition(13, 13, CheckOnField.White);
       // StartCheckPosition(3, 3, CheckOnField.White);
        //StartCheckPosition(11, 12, CheckOnField.White);
        //StartCheckPosition(30, 30, CheckOnField.Black);
	
	}
	
    /// <summary>
    /// Метод заполняет поля на доске шашками в начале игры
    /// </summary>
    /// <param name="startNum">Начальная позиция</param>
    /// <param name="endEnum">Конечная позиция</param>
    /// <param name="checkEnum">Цвет шашки</param>
    void StartCheckPosition(int startNum, int endEnum, CheckOnField checkEnum)
    {
        for(int i = startNum; i<=endEnum; i++)
        {
            field = GameObject.Find(i.ToString());

            Debug.Log("Состояние " + field.name+':'+field.GetComponent<FieldProperty>().OnFieldCheck); // В конце удалить
            
            if (checkEnum == CheckOnField.Black) {
                GameObject black;
                black = (GameObject)Instantiate(blackPrefab, new Vector3(0, 0, -3) + field.transform.position, Quaternion.identity);
                black.GetComponent<Move>().OverTheField = field;
            }

            if(checkEnum == CheckOnField.White)
            {
                GameObject white;
                white = (GameObject)Instantiate(whitePrefab, new Vector3(0, 0, -3) + field.transform.position, Quaternion.identity);
                white.GetComponent<Move>().OverTheField = field;
            }
            field.GetComponent<FieldProperty>().SetOnField(checkEnum);

            Debug.Log("Состояние " + field.name + ':' + field.GetComponent<FieldProperty>().OnFieldCheck); //В конце удалить
        }
    }
	
}
