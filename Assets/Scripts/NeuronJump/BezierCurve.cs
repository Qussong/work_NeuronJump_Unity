using System.Collections;
using System.Collections.Generic;
using System.Data;
using Radishmouse;
using UnityEditor;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public GameObject obj;
    public UILineRenderer UILineRenderer;
    [Range(0, 1)] public float progress;
    public List<GameObject> pointObjects = new List<GameObject>();
    private List<Vector2> points = new List<Vector2>();
    public int precision = 10;
    public bool bNeedPlayerInput = false;

    public void Start()
    {
        // Point ÁÂÇ¥ ÃßÃâ
        foreach(GameObject pointObj in pointObjects)
        {
            points.Add(pointObj.GetComponent<RectTransform>().anchoredPosition);
        }

        if (null == UILineRenderer) Debug.LogError("UI Line Renderer not set.");
        FineBezierPath();

    }

    public void Update()
    {
        obj.GetComponent<RectTransform>().localPosition = BezierTest(points[0], points[1], points[2], points[3], progress);
    }

    public void LateUpdate()
    {
        bool isMoved = false;
        for (int i = 0; i < points.Count; ++i)
        {
            Vector2 curPos = pointObjects[i].GetComponent<RectTransform>().anchoredPosition;
            if (curPos != points[i])
            {
                isMoved = true;
                points[i] = curPos;
            }
        }

        if (isMoved)
        {
            FineBezierPath();
            UILineRenderer.SetAllDirty();
        }
    }

    // Cubic Bezier Curve (3Â÷ º£Áö¾î°î¼±)
    public Vector3 BezierTest(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float value)
    {
        Vector2 A = Vector3.Lerp(p1, p2, value);
        Vector2 B = Vector3.Lerp(p2, p3, value);
        Vector2 C = Vector3.Lerp(p3, p4, value);

        Vector2 D = Vector3.Lerp(A, B, value);
        Vector2 E = Vector3.Lerp(B, C, value);

        Vector2 F = Vector3.Lerp(D, E, value);
        return F;
    }

    private void FineBezierPath()
    {
        List<Vector2> bezierRoutePoints = new List<Vector2>();
        for (int i = 0; i <= precision; i++)
        {
            Vector2 p = BezierTest(points[0], points[1], points[2], points[3], 1f / precision * i);
            bezierRoutePoints.Add(p);
        }
        UILineRenderer.points = bezierRoutePoints.ToArray();
    }

}