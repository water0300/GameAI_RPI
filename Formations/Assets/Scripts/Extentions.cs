using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extentions {
    static public Vector3 XYPlane(this Vector2 vec) => new Vector3(vec.x,vec.y,0f);

}
