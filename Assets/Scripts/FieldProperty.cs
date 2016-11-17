using UnityEngine;
using System.Collections;

public class FieldProperty : MonoBehaviour {

    /// <summary>
    /// Направления с этого поля
    /// </summary>
    [SerializeField]
    private DirectionEnum direction;

    /// <summary>
    /// Свойства для чтения текущего состояния поля
    /// </summary>
    public CheckOnField OnFieldCheck { get { return onField; } }
        

    /// <summary>
    /// Закрытая перменная состояния поля
    /// </summary>
    private CheckOnField onField;

    void Awake() {

        onField = CheckOnField.Empty;
    }

	
    /// <summary>
    /// Метод задает состояние поля.
    /// </summary>
    /// <param name="value">Значение цвета шашки</param>
    public void SetOnField(CheckOnField value)
    {
        onField = value;
    }
	
}
