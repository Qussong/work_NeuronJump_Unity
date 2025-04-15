using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


namespace GH
{
    public class Scroller : MonoBehaviour
    {
        [Tooltip("전체 경로 위를 움직일 객체")]
        [SerializeField] private GameObject moveTarget;
        [Tooltip("스크롤 뷰의 Scrool Rect 컴포넌트")]
        [SerializeField] private ScrollRect scrollRect;
        [Tooltip("시작 지점")]
        [SerializeField] GameObject startPoint;
        [Tooltip("끝 지점")]
        [SerializeField] GameObject endPoint;
        [Tooltip("경로를 이루는 Bezier Curve 들의 컨테이너")]
        [SerializeField] List<BezierCurve> curves = new List<BezierCurve>();
        [Tooltip("경로를 이루는 Beizer 곡선중 현재 위치한 곡선의 활성화에 대한 해시 테이블")]
        [SerializeField] List<bool> actives = new List<bool>();
        [Tooltip("경로를 이루는 Beizer 곡선중 현재 위치한 곡선의 인덱스")]
        [SerializeField] int curLevel = 0;
        //[Tooltip("게임 완료 여부")]
        //[SerializeField] bool bFinish = false;
        private float totalLength = 0f; // 전체 경로의 시작과 끝점의 Y차

        // Game
        [Tooltip("게임 시작 플래그")]
        [SerializeField] public bool bPlay = false; 
        [SerializeField] GameObject Ring;
        [Tooltip("점프 수행 뉴런 여부")]
        [SerializeField] List<bool> jumpRoutes = new List<bool>();
        [Tooltip("해당 뉴런에서 수행해야할 점프 완료 여부")]
        [SerializeField] bool bFinishJump = false;
        [Tooltip("점프 수행을 위해 멈추는 위치를 저장하는 컨테이너")]
        [SerializeField] List<float> stopPosRatios = new List<float>();
        [Tooltip("점프 수행 뉴런 인덱스")]
        [SerializeField] int jumpNeuronIdx = 0;
        [Tooltip("점프 마디 인덱스")]
        [SerializeField] int stopPointIdx = 0;

        void Start()
        {
            if (null == moveTarget) Debug.LogError("Move Target not set.");
            if (null == scrollRect) Debug.LogError("Scroll Rect not set.");
            if (0 == curves.Count) Debug.LogError("BezierCurve list not set.");
            if (null == startPoint || null == endPoint) Debug.LogError("Start/End Point not set.");

            Vector2 startPos = startPoint.GetComponent<RectTransform>().anchoredPosition;
            Vector2 endPos = endPoint.GetComponent<RectTransform>().anchoredPosition;
            totalLength = endPos.y - startPos.y;

            moveTarget.GetComponent<RectTransform>().anchoredPosition = startPos;

            for (int i = 0; i < curves.Count; i++) actives.Add(false);
            actives[0] = true;
        }

        // Update is called once per frame
        void Update()
        {
            Progress();
        }

        private void LateUpdate()
        {
            CalculateRatio();
        }

        public void Progress()
        {
            if(true == bPlay)
            {
                Scrolling();
                Trace();

                // 플레이어의 입력이 요구되는 상황
                if (true == actives[curLevel] || false == curves[curLevel].bNeedPlayerInput) return;
                bool bSuccessed = PlayerInput();
                LevelUp(bSuccessed);
            }
        }

        public void Scrolling()
        {
            if (true == NJ_ContentManager.Instance.IsGameEnd) return;
            if (true != actives[curLevel]) return;

            if (false == bFinishJump &&
                true == jumpRoutes[curLevel])
            {
                bool bPass = JumpNeuron(curves[curLevel].progress);
                if (false == bPass) return;
            }

            // 점프 수행 뉴런의 경우 천천히 이동
            if (true == jumpRoutes[curLevel])
                curves[curLevel].progress += Time.deltaTime / 5f;
            // 점프 수행 뉴런이 아닌경우 빨리 이동
            else
                curves[curLevel].progress += Time.deltaTime;


            if (curves[curLevel].progress >= 1)
            {
                actives[curLevel] = false;
                if (curLevel == curves.Count - 1) { NJ_ContentManager.Instance.IsGameEnd = true; return; }
                if (true == curves[curLevel].bNeedPlayerInput) return;
                actives[++curLevel] = true;
            }
        }

        public void Trace()
        {
            if (true == NJ_ContentManager.Instance.IsGameEnd) return;
            if (true != actives[curLevel]) return;

            Vector2 targetPos = curves[curLevel].obj.GetComponent<RectTransform>().anchoredPosition;
            moveTarget.GetComponent<RectTransform>().anchoredPosition = targetPos;
        }

        public void CalculateRatio()
        {
            if(true == bPlay)
            {
                Vector2 curTargetPos = moveTarget.GetComponent<RectTransform>().anchoredPosition;
                Vector2 startPos = startPoint.GetComponent<RectTransform>().anchoredPosition;
                float ratio = (curTargetPos.y - startPos.y) / totalLength;
                scrollRect.verticalNormalizedPosition = ratio;
            }
        }

        public void LevelUp(bool flag)
        {
            if (true == flag)
            {
                actives[++curLevel] = true;
                bFinishJump = false;
            }
        }

        public bool JumpNeuron(float posRatio)
        {
            int JUMP_CNT = 5;
            int stopPosRatioIdx = jumpNeuronIdx * JUMP_CNT + stopPointIdx;
            if (stopPointIdx == JUMP_CNT)
            {
                ++jumpNeuronIdx;
                stopPointIdx = 0;
                bFinishJump = true;
                return true;
            }

            float stopPosRatio = stopPosRatios[stopPosRatioIdx];
            float curPosRatio = Mathf.Clamp(posRatio, 0, stopPosRatio);

            if (curPosRatio == stopPosRatio)
            {
                if (PlayerInput())
                {
                    ++stopPointIdx;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public bool PlayerInput()
        {
            bool result = false;

            if (Input.GetKeyDown(KeyCode.Space))
                result = true;

            return result;
        }

    }

}

