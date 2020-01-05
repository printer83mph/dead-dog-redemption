using System;
using UnityEngine;

public static class PrintUtil
{
    public static Vector3 InputAxisTransform(float hor, float vert)
    {
        return new Vector3(hor * (float)Math.Sqrt(1 - Math.Pow(vert, 2) / 2), 0, vert * (float)Math.Sqrt(1 - Math.Pow(hor, 2) / 2));
    }
}