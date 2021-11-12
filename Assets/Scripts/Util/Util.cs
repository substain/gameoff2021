using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Util
{
    public enum Dir4
    {
        North, East, South, West
    }

    public static Vector3 ToVector3(Vector2 xzVector, float y = 0)
    {
        return new Vector3(xzVector.x, y, xzVector.y);
    }

    public static Vector2 ToVector2(Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Dir4 ToDir4(Vector2 vector)
    {
        float xValue = vector.x;
        float yValue = vector.y;
        if(Mathf.Abs(xValue) > Mathf.Abs(yValue)){
            return ToEastWestDir4(xValue);
        }
        else
        {
            return ToNorthSouthDir4(yValue);
        }
    }

    public static Dir4 ToNorthSouthDir4(float yValue)
    {
        return yValue > 0 ? Dir4.North : Dir4.South;
    }
    public static Dir4 ToEastWestDir4(float xValue)
    {
        return xValue > 0 ? Dir4.East : Dir4.West;
    }
}