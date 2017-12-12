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
            {
                fieldCollider.enabled = true;
                MoveTobit move = (MoveTobit)_moveOn;
                if (move.haveKill)
                {
                    fieldSprite.sprite = killMove;                    
                }
                else
                {
                    fieldSprite.sprite = normalMove;
                }
            }
            else
            {
                fieldCollider.enabled = false;
                fieldSprite.sprite = null;
            }
        }
    }

    public int row;
    public int column;

    [SerializeField]
    private Sprite normalMove;
    [SerializeField]
    private Sprite killMove;
    private Collider2D fieldCollider;
    private Move _moveOn;
    private SpriteRenderer fieldSprite;

    void Start()
    {
        fieldCollider = gameObject.GetComponent<Collider2D>();
        fieldSprite = gameObject.GetComponent<SpriteRenderer>();
        EventManager.Instance.AddListener(EVENT_TYPE.DEFAULT, OnEvent);
        MoveOn = null;
        SetFieldOnBoard();
    }
        
    private void SetFieldOnBoard()
    {
        GameController.Instance.deskView[row, column] = this;
    }
    
    public void OnEvent(EVENT_TYPE Event_type, GameObject Sender, object Param = null)
    {
        switch (Event_type)
        {
            case EVENT_TYPE.DEFAULT:
                MoveOn = null;
                break;
        }
    }

   
}
