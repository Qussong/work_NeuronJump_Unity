using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


namespace GH
{
    public class Scroller : MonoBehaviour
    {
        [Header("Scroll")]
        [Tooltip("스크롤 뷰의 Scroll Rect 컴포넌트")]
        [SerializeField] private ScrollRect scrollRect;
        [Tooltip("시작 지점")]
        [SerializeField] GameObject startPoint;
        [Tooltip("끝 지점")]
        [SerializeField] GameObject endPoint;
        [Tooltip("경로를 이루는 Bezier Curve 들의 컨테이너")]
        [SerializeField] public List<BezierCurve> curves = new List<BezierCurve>();
        [Tooltip("경로를 이루는 Beizer 곡선중 현재 위치한 곡선의 활성화에 대한 해시 테이블")]
        [SerializeField] List<bool> actives = new List<bool>();
        [Tooltip("경로를 이루는 Beizer 곡선중 현재 위치한 곡선의 인덱스")]
        [SerializeField] int curLevel;
        [Tooltip("전체 경로의 시작과 끝점의 Y차")]
        [ReadOnly][SerializeField] float totalLength;

        [Header("Game")]
        [Tooltip("전체 경로 위를 움직일 객체")]
        [SerializeField] private GameObject moveTarget;
        [Tooltip("게임 시작 플래그")]
        [ReadOnly][SerializeField] public bool bPlay;
        [Tooltip("점프 수행 뉴런 여부")]
        [SerializeField] List<bool> jumpRoutes = new List<bool>();
        [Tooltip("해당 뉴런에서 수행해야할 점프 완료 여부")]
        [ReadOnly][SerializeField] bool bFinishJump;
        [Tooltip("각 뉴런에서의 점프 횟수")]
        [ReadOnly][SerializeField] int maxJumpCnt;
        [Tooltip("점프 수행을 위해 멈추는 위치를 저장하는 컨테이너")]
        [SerializeField] List<float> stopPosRatios = new List<float>();
        [Tooltip("타겟이 현재 위치한 뉴런의 점프 뉴런 인덱스")]
        [ReadOnly][SerializeField] int curJumpNeuronIdx;
        [Tooltip("점프 마디 인덱스")]
        [ReadOnly][SerializeField] int stopPointIdx;
        [Tooltip("점프 이동 속도")]
        [ReadOnly][SerializeField] float jumpSpeed;

        void Start()
        {
            Init();
        }

        void Update()
        {
            Progress();
        }

        private void LateUpdate()
        {
            CalculateRatio();
        }

        public void Init()
        {
            if (null == scrollRect) Debug.LogError("Scroll Rect not set.");
            if (null == startPoint || null == endPoint) Debug.LogError("Start/End Point not set.");
            if (0 == curves.Count) Debug.LogError("경로를 이루는 베지어 곡선들이 설정되지 않았습니다.");
            if (0 == actives.Count) Debug.LogError("베지어 곡선 활성화 해시 테이블이 설정되지 않았습니다. (베이지어 곡선의 개수와 동일)");
            if (null == moveTarget) Debug.LogError("Move Target not set.");
            if (0 == jumpRoutes.Count) Debug.LogError("경로를 이루는 베지어 곡선 중 점프를 수행하는 곡선이 설정되지 않았습니다. (베이지어 곡선의 개수와 동일)");
            if (0 == stopPosRatios.Count) Debug.LogError("점프 수행을 위해 멈추는 위치가 설정되지 않았습니다.");

            //
            curLevel = 0;
            //
            Vector2 startPos = startPoint.GetComponent<RectTransform>().anchoredPosition;
            Vector2 endPos = endPoint.GetComponent<RectTransform>().anchoredPosition;
            totalLength = endPos.y - startPos.y;
            //
            bPlay = false;
            //
            bFinishJump = false;
            //
            int jumpRoutesCnt = 0;
            foreach (bool isJump in jumpRoutes) { if (true == isJump) ++jumpRoutesCnt; }
            maxJumpCnt = stopPosRatios.Count / jumpRoutesCnt;
            //
            curJumpNeuronIdx = 0;
            //
            stopPointIdx = 0;
            //
            jumpSpeed = 1f / 5f;
            // 각 베지어 커브의 ratio 0으로 위치 이동
            foreach (BezierCurve curve in curves) curve.progress = 0;
            //
            moveTarget.GetComponent<RectTransform>().anchoredPosition = startPos;
            //
            scrollRect.verticalNormalizedPosition = 0;
            //
            for (int i = 0; i < curves.Count; i++) actives[i] = false;
            actives[0] = true;
        }

        public void Progress()
        {
            if (true == bPlay)
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
            // 게임이 종료된 경우
            if (true == NJ_ContentManager.Instance.IsGameEnd) return;
            // 활성화된 베지어 곡선이 아닌 경우 (즉, 현재 뉴오가 위치해야할 뉴런이 아닌경우)
            if (true != actives[curLevel]) return;

            // 현재 뉴런에서 수행해야할 점프가 완료되지 않았고, 점프를 수행해야할 뉴런인 경우
            if (false == bFinishJump &&
                true == jumpRoutes[curLevel])
            {
                // 다음 위치로 이동
                bool bPass = JumpNeuron(curves[curLevel].progress);
                // 다음 위치로 이동하지 않은 경우 다음 위치로 이동하기 전까지 다음 로직으로 넘어가지 않음
                if (false == bPass) return;
            }

            // 점프 수행 뉴런의 경우 천천히 이동
            if (true == jumpRoutes[curLevel])
                curves[curLevel].progress += (Time.deltaTime * jumpSpeed);
            // 점프 수행 뉴런이 아닌경우 빨리 이동
            else
                curves[curLevel].progress += Time.deltaTime;

            if (curves[curLevel].progress >= 1)
            {
                actives[curLevel] = false;
                if (curLevel == curves.Count - 1)
                {
                    NJ_ContentManager.Instance.IsGameEnd = true;
                    curJumpNeuronIdx = 0;
                    return;
                }
                if (true == curves[curLevel].bNeedPlayerInput)
                {
                    return;
                }
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
            if (true == bPlay)
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
            int stopPosRatioIdx = curJumpNeuronIdx * maxJumpCnt + stopPointIdx;// 멈춤위치 인덱스
            float stopPosRatio = stopPosRatioIdx < stopPosRatios.Count ? stopPosRatios[stopPosRatioIdx] : 1;            // 멈춰야 할 스탑포인트 위치
            float curPosRatio = Mathf.Clamp(posRatio, 0, stopPosRatio);     // 현재 스탑 포인트 위치

            // 현재 스탑 포인트가 각 뉴런의 스탑포인트보다 커졌을 때
            if (stopPointIdx >= maxJumpCnt)
            {
                ++curJumpNeuronIdx;     // 뉴오가 위치한 뉴런의 인덱스 (다음 뉴런으로 넘어갔음을 의미함)
                stopPointIdx = 0;   // 다음 뉴런의 첫번째 스탑 포인트를 나타냄
                bFinishJump = true; // 해당 뉴런에서의 점프 완료 여부
                return true;
            }

            // 현재 스탑 포인트가 멈춰야 할 스탑포인트와 동일한 경우
            if (curPosRatio == stopPosRatio)
            {
                // 플레이어의 입력을 받아 점프를 수행한다.
                if (PlayerInput())
                {
                    ++stopPointIdx;
                    if (true == jumpRoutes[curLevel])
                    {
                        int nextStopPosRatioIdx = stopPosRatioIdx + 1;
                        float nextStopPosRatio = 1;
                        if (0 != nextStopPosRatioIdx % 5) nextStopPosRatio = stopPosRatios[stopPosRatioIdx + 1];
                        RunZoomInOutEffect(nextStopPosRatio - curPosRatio);
                    }
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
            {
                result = true;
            }

            return result;
        }

        private void RunZoomInOutEffect(float moveRatio)
        {
            float duration = moveRatio / jumpSpeed;
            moveTarget.GetComponent<ZoomInOut>().RunEffect(duration);
        }
    }

}

