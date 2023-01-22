using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AgentArrowIndicator : MonoBehaviour
{
    [Header("Options")]
    public Color color;
    public float lineWidth = 0.2f;
    public Vector3 Vector {get; set;} = Vector3.zero;
    private LineRenderer _lineRenderer;
    private void Awake() {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start() {
        SetupArrow();
    }

    private void LateUpdate() {
        DrawArrow();
    }

    private void SetupArrow(){
        _lineRenderer.startWidth = lineWidth;
        _lineRenderer.endWidth = lineWidth;
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }

    private Vector3 _lastVector = Vector3.zero;
    private Vector3 _v;
    private void DrawArrow(){
        _lineRenderer.SetPosition(0, transform.position);
        Vector3 newVec = Vector3.SmoothDamp(_lastVector, Vector, ref _v, 0.5f);
        _lineRenderer.SetPosition(1,  transform.position + newVec);
        _lastVector = newVec;
    }


}
