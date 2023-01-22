using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AgentArrowIndicator : MonoBehaviour
{
    [Header("Options")]
    public Color color;
    public float lineWidth = 0.2f;
    public Vector3 Vector {get; set;}
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
        DrawArrow();
    }

    private void DrawArrow(){
        // float deltaTheta = (2f*Mathf.PI) / vertexCount;
        // float theta = 0f;
        // for(int i = 0; i < vertexCount; i++){
        //     _lineRenderer.SetPosition(i, transform.position + new Vector3(Radius * Mathf.Cos(theta), 0f, Radius * Mathf.Sin(theta)));
        //     theta += deltaTheta;
        // }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, transform.position + Vector);
    }
}
