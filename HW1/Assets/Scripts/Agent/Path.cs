using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Path {
    //let param major = segment #
    //let param minor = lerped segment
    public List<PathSegmentData> Segments {get; private set; } = new List<PathSegmentData>();

    private Vector3 GetClosestSegmentPoint(Vector3 agentPos, int paramMajor){
        return Utilities.FindNearestPointOnLine(Segments[paramMajor].start, Segments[paramMajor].end, agentPos);
    }

    // public float GetParam(Vector3 agentPos){
    //     int paramMajor = Segments.Select((seg, index) => index).Aggregate((l, r) => GetClosestSegmentPoint(agentPos, l).sqrMagnitude < GetClosestSegmentPoint(agentPos, r).sqrMagnitude ? l : r);
    //     float paramMinor = Utilities.InverseLerp(Segments[(int)paramMajor].start, Segments[(int)paramMajor].end, GetClosestSegmentPoint(agentPos, paramMajor));
    //     return paramMajor + paramMinor;    
    // }
    public float GetParam(Vector3 agentPos, float lastParam){
        int paramMajor = (int) lastParam;
       
        Vector3 closest = GetClosestSegmentPoint(agentPos, paramMajor);

        float paramMinor = Utilities.InverseLerp(Segments[paramMajor].start, Segments[paramMajor].end, closest) * 1.1f - 0.1f; //todo - hardcoded to allow for multi direction
        Debug.Log($"Last: {paramMajor}, New: {paramMinor}");
        //this is hard coded - fix it later
        return Mathf.Clamp(paramMajor + paramMinor, 0, Segments.Count - 0.01f);
    }

    public Vector3 GetTargetPosition(float param){
        if(param < 0){
            // Debug.Log("yewot < 0");
            return Segments[0].start;
        }
        if(param >= Segments.Count){
            // Debug.Log("yetwot > c");
            return Segments[Segments.Count-1].end;
        }

        // Debug.Log($"new param: {param}");
        return Vector3.Lerp(Segments[(int)param].start, Segments[(int)param].end, param - (int)param);
    }
}

