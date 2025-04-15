using System.Collections;
using System.Collections.Generic;
using System.Data;
using Radishmouse;
using UnityEditor;
using UnityEngine;


namespace GH
{
    public class BezierCurve : MonoBehaviour
    {
        [Tooltip("라인을 따라 움직이는 객체")]
        [SerializeField] public GameObject obj;
        [Tooltip("경로를 그려주는 UI Line Renderer 컴포넌트")]
        [SerializeField] public UILineRenderer UILineRenderer;
        [Tooltip("진행도 (0 ~ 1)")]
        [Range(0, 1)] public float progress;
        [Tooltip("경로를 구성하는 포인트 객체들을 저장하는 컨테이너 (Pq ~ P4)")]
        [SerializeField] public List<GameObject> pointObjects = new List<GameObject>();
        [Tooltip("경로를 이루는 점들을 저장하는 컨테이너")]
        [SerializeField] private List<Vector2> points = new List<Vector2>();
        [Tooltip("경로의 정밀도 (얼마나 세세하게 점을 추출할지)")]
        [SerializeField] public int precision = 10;

        [Header("Game For Content")]
        [Tooltip("다음 경로를 넘어가기 위해 플레이어의 입력이 요구되는지 확인하는 플래그")]
        public bool bNeedPlayerInput = false;

        public void Start()
        {
            // Point 좌표 추출
            foreach (GameObject pointObj in pointObjects)
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

        // Cubic Bezier Curve (3차 베지어곡선)
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
}

