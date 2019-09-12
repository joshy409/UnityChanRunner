using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class LocationDecode {

    //OFFSET needs to be a even number;
    public const int OFFSET = 20;

    public static List<Vector3> locationToLocalPos = new List<Vector3>()
    {
        new Vector3(-OFFSET, -OFFSET, 0) ,
        new Vector3(0, -OFFSET, 0) ,
        new Vector3(OFFSET, -OFFSET, 0) ,
        new Vector3(-OFFSET, 0, 0) ,
        new Vector3(0, 0, 0) ,
        new Vector3(OFFSET, 0, 0) ,
        new Vector3(-OFFSET, OFFSET, 0) ,
        new Vector3(0, OFFSET, 0) ,
        new Vector3(OFFSET, OFFSET, 0) 
    };

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down,
        Forward
    }

    public enum PathType
    {
        Straight,
        Turn,
        Up,
        Down,
        UpFliped,
        DownFliped
    }


}
