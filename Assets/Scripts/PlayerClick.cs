using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClick : MonoBehaviour {
    
    
    private static ViewFigure currentFigure = null;


    void OnMouseDown()
    {        
        if(gameObject.tag == "Field")
        {
            Field fieldToMove = gameObject.GetComponent<Field>();
            MoveTobit currentMove = (MoveTobit)fieldToMove.MoveOn;
            currentMove.figure.Move(currentMove, ref GameController.Instance.CurrentBoard.board);
            EventManager.Instance.PostNotification(EVENT_TYPE.DEFAULT);
            GameController.Instance.ChangePlayer();
        }
        if(gameObject.tag == "Figure")
        {
            ViewFigure figure = gameObject.GetComponent<ViewFigure>();
            if (figure.figure.color == GameController.Instance.playerColor)
            {
                if(currentFigure != figure)
                {                    
                    EventManager.Instance.PostNotification(EVENT_TYPE.DEFAULT);                    
                }
                currentFigure = figure;
                Move[] moves = figure.figure.GetMoves(ref GameController.Instance.CurrentBoard.board);
                GameController.Instance.HighLightMoves(moves);
            }
        }
    }


}
