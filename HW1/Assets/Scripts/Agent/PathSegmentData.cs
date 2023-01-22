using UnityEngine;

public struct PathSegmentData {
    public Vector3 start;
    public Vector3 end;

    public PathSegmentData(Vector3 start, Vector3 end){
        this.start = start;
        this.end = end;
    }
}
