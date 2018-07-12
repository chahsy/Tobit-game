using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewFigure : MonoBehaviour {

    public Figure figure;
    public Sprite black;
    public Sprite white;
    public Sprite superBlack;
    public Sprite superWhite;
    public Collider2D coll2D;

    private void Awake()
    {
        figure = new Figure();
        coll2D = GetComponent<Collider2D>();
    }

    public void SetStartProperties(Figure figure)
    {        
        coll2D.enabled = true;
        this.figure = figure;
        if (figure.Color == FigureColor.WHITE)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = white;
            gameObject.name = "White";
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = black;
            gameObject.name = "Black";
        }
        this.figure.MovedEvent += MoveFigure;
        this.figure.DestroyEvent += OnFigureDestoy;
        this.figure.BecameSuperEvent += ChangeSprite;

    }

    public void MoveFigure(MoveTobit move)
    {
        Vector3 pos = GameController.Instance.deskView[move.row, move.col].transform.position;
        gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z - 1);

    }
    public void OnFigureDestoy()
    {
        Destroy(gameObject);
    }

    private void ChangeSprite()
    {
        if (figure.Color == FigureColor.WHITE)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = superWhite;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = superBlack;
        }
    }
}
