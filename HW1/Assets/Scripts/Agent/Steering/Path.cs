using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Path {
    //let param major = segment #
    //let param minor = lerped segment
    public List<PathSegment> Segments {get; private set; } = new List<PathSegment>();

    private Vector3 GetClosestSegmentPoint(Vector3 agentPos, float paramMajor){
        return Utilities.FindNearestPointOnLine(Segments[(int)paramMajor].start, Segments[(int)paramMajor].end, agentPos);
    }

    public float GetParam(Vector3 agentPos){
        int paramMajor = Segments.Select((seg, index) => index).Aggregate((l, r) => GetClosestSegmentPoint(agentPos, l).sqrMagnitude < GetClosestSegmentPoint(agentPos, r).sqrMagnitude ? l : r);
        float paramMinor = Utilities.InverseLerp(Segments[(int)paramMajor].start, Segments[(int)paramMajor].end, GetClosestSegmentPoint(agentPos, paramMajor));
        return paramMajor + paramMinor;    
    }
    public float GetParam(Vector3 agentPos, float lastParam){
        Debug.Log($"last param: {lastParam}");

        if(lastParam < 0 || lastParam > Segments.Count-1){
            return lastParam;
        }

        Dictionary<float, Vector3> distGroups = new Dictionary<float, Vector3>();
        if(lastParam != 0){
            distGroups.Add(lastParam-1, GetClosestSegmentPoint(agentPos, lastParam-1));

        }
        if(lastParam != Segments.Count-1){
            distGroups.Add(lastParam+1, GetClosestSegmentPoint(agentPos, lastParam+1));
        } 
        distGroups.Add(lastParam, GetClosestSegmentPoint(agentPos, lastParam));
        float paramMajor = distGroups.Aggregate((l, r) => l.Value.sqrMagnitude < r.Value.sqrMagnitude ? l : r).Key;
        float paramMinor = Utilities.InverseLerp(Segments[(int)paramMajor].start, Segments[(int)paramMajor].end, distGroups[paramMajor]);

        return paramMajor + paramMinor;
    }

    public Vector3 GetTargetPosition(float param){
        if(param < 0){
            // Debug.Log("yewot < 0");
            return Segments[0].start;
        }
        if(param > Segments.Count){
            // Debug.Log("yetwot > c");
            return Segments[Segments.Count-1].end;
        }

        // Debug.Log($"new param: {param}");
        return Vector3.Lerp(Segments[(int)param].start, Segments[(int)param].end, param - (int)param);
    }
}

public struct PathSegment {
    public Vector3 start;
    public Vector3 end;

    public PathSegment(Vector3 start, Vector3 end){
        this.start = start;
        this.end = end;
    }
}