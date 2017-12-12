using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    

    void OnMouseDown()
    {
        float eval = 1f;
        float pointSimple = 1f;
        float pointSuccess = 10f;
        ViewFigure figure = gameObject.GetComponent<ViewFigure>();
        if (figure.figure.color == FigureColor.BLACK)
        {
            Move[] moves = figure.figure.GetMoves(ref GameController.Instance.CurrentBoard.board);
            int t = 0;
            foreach (Move mv in moves)
            {
                t++;
                MoveTobit m = (MoveTobit)mv;
                if (m.haveKill)
                {
                    eval += pointSuccess;
                }
                else
                {
                    eval += pointSimple;
                }
                if (IsImrpovedMove(GameController.Instance.activePlayer, m))
                    eval += pointSuccess;
                Debug.Log("Ход №" + t + " = " + eval);
            }
        }
    }

    private bool IsImrpovedMove(FigureColor color, MoveTobit m)
    {
        int rows = GameController.Instance.CurrentBoard.board.GetLength(0) - 1;
        if (color == FigureColor.WHITE && m.y == rows)
        {
            return true;
        }
        if (color == FigureColor.BLACK && m.y == 0)
        {
            return true;

        }
        return false;
    }
}