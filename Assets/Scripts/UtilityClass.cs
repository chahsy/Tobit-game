using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UtilityClass : MonoBehaviour, IGameManager {

    private const int MaxFieldOnBoard = 38;

    int layerMask = 1 << 8;

    public ManagerStatus status { get; private set; }

    

    public void Startup()
    {
        StartCoroutine(DoDictionaries());        
        status = ManagerStatus.Started;
    }

    public IEnumerator DoDictionaries()
    {
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Field").Length == MaxFieldOnBoard);
             
        
        for (int i = 1; i <= MaxFieldOnBoard; i++)
        {
            Dictionary<DirectionEnum, List<Field>> longBufferDict = new Dictionary<DirectionEnum, List<Field>>(); 
            Dictionary<DirectionEnum, Field> shortBufferDict = new Dictionary<DirectionEnum, Field>();            
            GameObject field = GameObject.Find(i.ToString());            
            if (OnlyUpFields(i))
            {
                List<Field> bufferLines = new List<Field>();
                RaycastHit2D[] figuresOnLine = Physics2D.RaycastAll(field.transform.position, Vector2.up, Mathf.Infinity, layerMask);
                foreach (RaycastHit2D figure in figuresOnLine)
                {
                    if (figure.collider.gameObject.name != field.name)
                    {                        
                        Field currentField = figure.collider.gameObject.GetComponent<Field>();
                        bufferLines.Add(currentField);
                    }
                }               
                shortBufferDict.Add(DirectionEnum.Up, bufferLines.First());
                longBufferDict.Add(DirectionEnum.Up, bufferLines);
                field.GetComponent<Field>().Neighbors = shortBufferDict;
                field.GetComponent<Field>().TobitNeighbors = longBufferDict;

            }
            else if (OnlyDownFields(i))
            {
                List<Field> bufferLines = new List<Field>();
                RaycastHit2D[] figuresOnLine = Physics2D.RaycastAll(field.transform.position, Vector2.down, Mathf.Infinity, layerMask);
                foreach (RaycastHit2D figure in figuresOnLine)
                {
                    if (figure.collider.gameObject.name != field.name)
                    {                        
                        Field currentField = figure.collider.gameObject.GetComponent<Field>();
                        bufferLines.Add(currentField);
                    }

                }
                shortBufferDict.Add(DirectionEnum.Down, bufferLines.First());
                longBufferDict.Add(DirectionEnum.Down, bufferLines);
                field.GetComponent<Field>().Neighbors = shortBufferDict;
                field.GetComponent<Field>().TobitNeighbors = longBufferDict;
            }
            else if (OnlyRightFields(i))
            {
                List<Field> bufferLines = new List<Field>();
                RaycastHit2D[] figuresOnLine = Physics2D.RaycastAll(field.transform.position, Vector2.right, Mathf.Infinity, layerMask);
                foreach (RaycastHit2D figure in figuresOnLine)
                {
                    if (figure.collider.gameObject.name != field.name)
                    {
                        Field currentField = figure.collider.gameObject.GetComponent<Field>();
                        bufferLines.Add(currentField);
                    }

                }
                shortBufferDict.Add(DirectionEnum.Right, bufferLines.First());
                longBufferDict.Add(DirectionEnum.Right, bufferLines);
                field.GetComponent<Field>().Neighbors = shortBufferDict;
                field.GetComponent<Field>().TobitNeighbors = longBufferDict;
            }
            else if (OnlyLeftFields(i))
            {
                List<Field> bufferLines = new List<Field>();
                RaycastHit2D[] figuresOnLine = Physics2D.RaycastAll(field.transform.position, Vector2.left, Mathf.Infinity, layerMask);
                foreach (RaycastHit2D figure in figuresOnLine)
                {
                    if (figure.collider.gameObject.name != field.name)
                    {                        
                        Field currentField = figure.collider.gameObject.GetComponent<Field>();
                        bufferLines.Add(currentField);
                    }

                }
                shortBufferDict.Add(DirectionEnum.Left, bufferLines.First());
                longBufferDict.Add(DirectionEnum.Left, bufferLines);
                field.GetComponent<Field>().Neighbors = shortBufferDict;
                field.GetComponent<Field>().TobitNeighbors = longBufferDict;
            }
            else
            {
                Vector2[] directionVectors = { Vector2.up, Vector2.down, Vector2.right, Vector2.left };
                foreach (Vector2 dir in directionVectors)
                {                    
                    List<Field> bufferLines = new List<Field>();
                    RaycastHit2D[] figuresOnLine = Physics2D.RaycastAll(field.transform.position, dir, Mathf.Infinity, layerMask);
                    foreach(RaycastHit2D figure in figuresOnLine)
                    {
                        if (figure.collider.gameObject.name != field.name)
                        {                            
                            Field currentField = figure.collider.gameObject.GetComponent<Field>();
                            bufferLines.Add(currentField);
                        }

                    }
                    shortBufferDict.Add(IsDirection(dir), bufferLines.First());
                    longBufferDict.Add(IsDirection(dir), bufferLines);
                }               

                field.GetComponent<Field>().Neighbors = shortBufferDict;
                field.GetComponent<Field>().TobitNeighbors = longBufferDict;
            }
        }

        Debug.Log("Соседи для полей установлены!");
        EventManager.Instance.PostNotification(EVENT_TYPE.DEFAULT);

    }

    private DirectionEnum IsDirection(Vector2 dir)
    {
        DirectionEnum currentDir = DirectionEnum.Nothing;
        if(dir == Vector2.up)
        {
            currentDir = DirectionEnum.Up;
        }
        if(dir == Vector2.down)
        {
            currentDir = DirectionEnum.Down;
        }
        if(dir == Vector2.right)
        {
            currentDir = DirectionEnum.Right;
        }
        if (dir == Vector2.left)
        {
            currentDir = DirectionEnum.Left;
        }
        return currentDir;
    }

    private bool OnlyLeftFields(int numField)
    {        
        if(numField == 12 || numField == 19 || numField == 26 || numField == 33)
        {
            return true;
        }
        return false;
    }

    private bool OnlyRightFields(int numField)
    {
        if (numField == 6 || numField == 13 || numField == 20 || numField == 27)
        {
            return true;
        }
        return false;
    }

    private bool OnlyDownFields(int numField)
    {
        bool IsDownField = false;
        for(int i = 34; i <=38; i++)
        {
            if (i == numField)
            {
                IsDownField = true;
            }
        }
        return IsDownField;
    }

    private bool OnlyUpFields(int numField)
    {
        bool IsDownField = false;
        for (int i = 1; i <= 5; i++)
        {
            if (i == numField)
            {
                IsDownField = true;
            }
        }
        return IsDownField;
    }
}
