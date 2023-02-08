using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extentions {
    static public Vector3 XYPlane(this Vector2 vec) => new Vector3(vec.x,vec.y,0f);
    static public Vector2 IgnoreZ(this Vector3 vec) => new Vector2(vec.x,vec.y);

}

public static class Utilities {
    // static public Vector2 DirFromAngle(this Vector2 pos, float angle){

    // }
}
