using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerClick : MonoBehaviour {
    
    
    private static ViewFigure currentFigure = null;


    void OnMouseDown()
    {        
        if(gameObject.tag == "Field")
        {
            Field fieldToMove = gameObject.GetComponent<Field>();
            MoveTobit currentMove = (MoveTobit)fieldToMove.MoveOn;
            currentMove.figure.MovePlayer(currentMove, ref GameController.Instance.CurrentBoard.board);
            EventManager.Instance.PostNotification(EVENT_TYPE.DEFAULT);
            if (currentMove.haveKill && currentFigure.figure.GetMoves(ref GameController.Instance.CurrentBoard.board).ToList().Exists(x => (x as MoveTobit).haveKill))
            {
                Move[] moves = currentFigure.figure.GetMoves(ref GameController.Instance.CurrentBoard.board);
                GameController.Instance.HighLightMoves(moves);
            }
            else
            {
                GameController.Instance.ChangePlayer();
            }

        }
        if(gameObject.tag == "Figure")
        {
            ViewFigure selectedFigure = gameObject.GetComponent<ViewFigure>();
            if (selectedFigure.figure.Color == GameController.Instance.playerColor)
            {
                if(currentFigure != selectedFigure)
                {                    
                    EventManager.Instance.PostNotification(EVENT_TYPE.DEFAULT);                    
                }
                currentFigure = selectedFigure;
                Move[] moves = selectedFigure.figure.GetMoves(ref GameController.Instance.CurrentBoard.board);
                GameController.Instance.HighLightMoves(moves);
            }
        }
    }


}
