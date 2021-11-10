using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Util
{
    public static Vector3 ToVector3(Vector2 xzVector, float y = 0)
    {
        return new Vector3(xzVector.x, y, xzVector.y);
    }

}