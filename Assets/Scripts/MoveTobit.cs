using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTobit : Move {

    public int x;
    public int y;
    public bool haveKill;
    public int delX;
    public int delY;
    public Figure figure;

    public override void InnerData()
    {
        Debug.Log(
            "x= " + x+"\n"+
            "y= " + y + "\n" +
            "figureX= " + figure.x + "\n" +
            "figureY= " + figure.y + "\n"
            );
    }
}
