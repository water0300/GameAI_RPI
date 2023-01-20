using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Path {
    //let param major = segment #
    //let param minor = lerped segment
    public List<PathSegment> Segments {get; private set; } = new List<PathSegment>();

    private Vector3 GetClosestSegmentPoint(Vector3 agentPos, float param){
        return (agentPos - Utilities.FindNearestPointOnLine(Segments[(int)param].start, Segments[(int)param].start, agentPos));
    }
    public float GetParam(Vector3 agentPos, float lastParam){
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

    public Vector3 GetPosition(float param){
        if(param <= 0){
            return Segments[0].start;
        }
        if(param >= Segments.Count-1){
            return Segments[Segments.Count-1].end;
        }

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