using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public int row;
    public int column;
       

    void Start()
    {
        SetFieldOnBoard();
    }


    private void SetFieldOnBoard()
    {
        BoardTobit.Instance.fieldsOnBoard[row, column] = this;
    }
}
