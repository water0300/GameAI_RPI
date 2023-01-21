using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AgentCircleIndicator : MonoBehaviour
{
    [Header("Options")]
    public Color color;
    public int vertexCount = 40;
    public float lineWidth = 0.2f;
    public float Radius {get; set;}
    public Vector3 Offset {get; set; } = Vector3.zero;
    private LineRenderer _lineRenderer;
    private void Awake() {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start() {
        SetupCircle();
    }

    private void LateUpdate() {
        DrawCircle();
    }

    private void SetupCircle(){
        _lineRenderer.startWidth = lineWidth;
        _lineRenderer.endWidth = lineWidth;
        _lineRenderer.positionCount = vertexCount;
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
        DrawCircle();
    }

    private void DrawCircle(){
        float deltaTheta = (2f*Mathf.PI) / vertexCount;
        float theta = 0f;
        for(int i = 0; i < vertexCount; i++){
            _lineRenderer.SetPosition(i, transform.position + Offset + new Vector3(Radius * Mathf.Cos(theta), 0f, Radius * Mathf.Sin(theta)));
            theta += deltaTheta;
        }
    }

    private void OnDrawGizmos() {
        float deltaTheta = (2f*Mathf.PI) / vertexCount;
        float theta = 0f;
        Vector3 oldPos = transform.position;
        for(int i = 0; i < vertexCount; i++){
            Vector3 pos = new Vector3(Radius * Mathf.Cos(theta), 0f, Radius * Mathf.Sin(theta)) + Offset;
            Gizmos.DrawLine(oldPos, transform.position + pos);
            oldPos = transform.position + pos;
            theta += deltaTheta;
        }
    }
}
