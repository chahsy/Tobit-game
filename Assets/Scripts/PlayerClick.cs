using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClick : MonoBehaviour {

    private static Figure currentFigureForMove = null;
    private Move onFieldMove = null; // реализовать через свойство, если не нулл то включить коллайдер

    


    void OnMouseDown()
    {
        if(gameObject.GetComponent<Figure>() != null)
        {    
            Figure curFigure = gameObject.GetComponent<Figure>();
            Move[] moveFrom = curFigure.GetMoves(ref BoardTobit.Instance.board);
            Debug.Log(moveFrom.Length);
            if(moveFrom.Length > 0)
            {                
                currentFigureForMove = curFigure;
                Debug.Log("Click");               
                foreach (MoveTobit m in moveFrom)
                {
                    if (m.haveKill)
                        Debug.Log(BoardTobit.Instance.fieldsOnBoard[m.y, m.x].name + " подсвечен красным!");
                    else
                        Debug.Log(BoardTobit.Instance.fieldsOnBoard[m.y, m.x].name + " подсвечен желтым!");

                }
            }
        }
    }
}
