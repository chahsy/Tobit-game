using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public Move MoveOn
    {
        get
        {
            return _moveOn;
        }
        set
        {
            _moveOn = value;
            if (_moveOn != null)
                fieldCollider.enabled = true;
            else
                fieldCollider.enabled = false;
        }
    }

    public int row;
    public int column;

    private Collider2D fieldCollider;
    private Move _moveOn;

    void Start()
    {
        fieldCollider = gameObject.GetComponent<Collider2D>();
        MoveOn = null;
        SetFieldOnBoard();
    }

    private void OnMouseDown()
    {
        if(MoveOn != null)
        {
            MoveTobit curretMove = (MoveTobit)MoveOn;
            curretMove.figure.Move(curretMove, transform.position ,ref BoardTobit.Instance.board);
        }
    }


    private void SetFieldOnBoard()
    {
        BoardTobit.Instance.fieldsOnBoard[row, column] = this;
    }
}
