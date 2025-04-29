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
        [Tooltip("��ũ�� ���� Scroll Rect ������Ʈ")]
        [SerializeField] private ScrollRect scrollRect;
        [Tooltip("���� ����")]
        [SerializeField] GameObject startPoint;
        [Tooltip("�� ����")]
        [SerializeField] GameObject endPoint;
        [Tooltip("��θ� �̷�� Bezier Curve ���� �����̳�")]
        [SerializeField] public List<BezierCurve> curves = new List<BezierCurve>();
        [Tooltip("��θ� �̷�� Beizer ��� ���� ��ġ�� ��� Ȱ��ȭ�� ���� �ؽ� ���̺�")]
        [SerializeField] List<bool> actives = new List<bool>();
        [Tooltip("��θ� �̷�� Beizer ��� ���� ��ġ�� ��� �ε���")]
        [SerializeField] int curLevel;
        [Tooltip("��ü ����� ���۰� ������ Y��")]
        [ReadOnly][SerializeField] float totalLength;

        [Header("Game")]
        [Tooltip("��ü ��� ���� ������ ��ü")]
        [SerializeField] private GameObject moveTarget;
        [Tooltip("�÷��̾��� �Է� Ÿ�̹��� �˷��� ��ü")]
        [SerializeField] private GameObject inputTimingCircle;
        private float scaleElapsedTime;
        [Tooltip("���� ���� �÷���")]
        [ReadOnly][SerializeField] public bool bPlay;
        [Tooltip("���� ���� ���� ����")]
        [SerializeField] List<bool> jumpRoutes = new List<bool>();
        [Tooltip("�ش� �������� �����ؾ��� ���� �Ϸ� ����")]
        [ReadOnly][SerializeField] bool bFinishJump;
        [Tooltip("�� ���������� ���� Ƚ��")]
        [ReadOnly][SerializeField] int maxJumpCnt;
        [Tooltip("���� ������ ���� ���ߴ� ��ġ�� �����ϴ� �����̳�")]
        [SerializeField] List<float> stopPosRatios = new List<float>();
        [Tooltip("Ÿ���� ���� ��ġ�� ������ ���� ���� �ε���")]
        [ReadOnly][SerializeField] int curJumpNeuronIdx;
        [Tooltip("���� ���� �ε���")]
        [ReadOnly][SerializeField] int stopPointIdx;
        [Tooltip("���� �̵� �ӵ�")]
        [ReadOnly][SerializeField] float jumpSpeed;

        [Header("Serial")]
        [ReadOnly][SerializeField] bool isPlayerInput;

        public bool IsPlayerInput
        {
            get { return isPlayerInput; }
            set { isPlayerInput = value; }
        }

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
            if (0 == curves.Count) Debug.LogError("��θ� �̷�� ������ ����� �������� �ʾҽ��ϴ�.");
            if (0 == actives.Count) Debug.LogError("������ � Ȱ��ȭ �ؽ� ���̺��� �������� �ʾҽ��ϴ�. (�������� ��� ������ ����)");
            if (null == moveTarget) Debug.LogError("Move Target not set.");
            if (null == inputTimingCircle) Debug.LogError("�÷��̾��� ���� Ÿ�̹��� �˷��� ������ü�� �������� �ʾҽ��ϴ�.");
            inputTimingCircle.GetComponent<RectTransform>().localScale = Vector3.zero;
            if (0 == jumpRoutes.Count) Debug.LogError("��θ� �̷�� ������ � �� ������ �����ϴ� ��� �������� �ʾҽ��ϴ�. (�������� ��� ������ ����)");
            if (0 == stopPosRatios.Count) Debug.LogError("���� ������ ���� ���ߴ� ��ġ�� �������� �ʾҽ��ϴ�.");

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
            jumpSpeed = 1f / 4f;
            // �� ������ Ŀ���� ratio 0���� ��ġ �̵�
            foreach (BezierCurve curve in curves) curve.progress = 0;
            //
            moveTarget.GetComponent<RectTransform>().anchoredPosition = startPos;
            //
            scrollRect.verticalNormalizedPosition = 0;
            //
            for (int i = 0; i < curves.Count; i++) actives[i] = false;
            actives[0] = true;
            //
            isPlayerInput = false;
            //
            scaleElapsedTime = 0f;
        }

        public void Progress()
        {
            if (true == bPlay)
            {
                Scrolling();
                Trace();

                inputTimingCircle.GetComponent<RectTransform>().localPosition = moveTarget.GetComponent<RectTransform>().localPosition;

                // �÷��̾��� �Է��� �䱸�Ǵ� ��Ȳ
                if (true == actives[curLevel] || false == curves[curLevel].bNeedPlayerInput) return;

                float curScale = CircleEffect();
                if(PlayerInput())
                {
                    if (curScale >= 0.6 && curScale <= 0.8)
                    {
                        inputTimingCircle.GetComponent<RectTransform>().localScale = Vector3.zero;
                        scaleElapsedTime = 0f;
                        LevelUp();
                    }
                }
            }
        }

        public void Scrolling()
        {
            // ������ ����� ���
            if (true == NJ_ContentManager.Instance.IsGameEnd) return;
            // Ȱ��ȭ�� ������ ��� �ƴ� ��� (��, ���� ������ ��ġ�ؾ��� ������ �ƴѰ��)
            if (true != actives[curLevel]) return;

            // ���� �������� �����ؾ��� ������ �Ϸ���� �ʾҰ�, ������ �����ؾ��� ������ ���
            if (false == bFinishJump &&
                true == jumpRoutes[curLevel])
            {
                // ���� ��ġ�� �̵�
                bool bPass = JumpNeuron(curves[curLevel].progress);
                // ���� ��ġ�� �̵����� ���� ��� ���� ��ġ�� �̵��ϱ� ������ ���� �������� �Ѿ�� ����
                if (false == bPass) return;
            }

            // ���� ���� ������ ��� õõ�� �̵�
            if (true == jumpRoutes[curLevel])
                curves[curLevel].progress += (Time.deltaTime * jumpSpeed);
            // ���� ���� ������ �ƴѰ�� ���� �̵�
            else
                curves[curLevel].progress += Time.deltaTime ;

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

        public void LevelUp()
        {
            actives[++curLevel] = true;
            bFinishJump = false;
        }

        public bool JumpNeuron(float posRatio)
        {
            int stopPosRatioIdx = curJumpNeuronIdx * maxJumpCnt + stopPointIdx;// ������ġ �ε���
            float stopPosRatio = stopPosRatioIdx < stopPosRatios.Count ? stopPosRatios[stopPosRatioIdx] : 1;    // ����� �� ��ž����Ʈ ��ġ
            float curPosRatio = Mathf.Clamp(posRatio, 0, stopPosRatio);     // ���� ��ž ����Ʈ ��ġ

            // ���� ��ž ����Ʈ�� �� ������ ��ž����Ʈ���� Ŀ���� ��
            if (stopPointIdx >= maxJumpCnt)
            {
                ++curJumpNeuronIdx; // ������ ��ġ�� ������ �ε��� (���� �������� �Ѿ���� �ǹ���)
                stopPointIdx = 0;   // ���� ������ ù��° ��ž ����Ʈ�� ��Ÿ��
                bFinishJump = true; // �ش� ���������� ���� �Ϸ� ����
                return true;
            }

            // ���� ��ž ����Ʈ�� ����� �� ��ž����Ʈ�� ������ ���
            if (curPosRatio == stopPosRatio)
            {
                // �� ���� �� ���
                float curScale = CircleEffect();
                // �÷��̾��� �Է��� �޾� ������ �����Ѵ�.
                if (PlayerInput())
                {
                    // �˸��� Ÿ�̹� (scale : 0.6 ~ 0.8)
                    if(curScale >= 0.6 && curScale <= 0.8)
                    {
                        ++stopPointIdx;
                        if (true == jumpRoutes[curLevel])
                        {
                            int nextStopPosRatioIdx = stopPosRatioIdx + 1;
                            float nextStopPosRatio = 1;
                            if (0 != nextStopPosRatioIdx % 5) nextStopPosRatio = stopPosRatios[stopPosRatioIdx + 1];
                            RunZoomInOutEffect(nextStopPosRatio - curPosRatio);
                            inputTimingCircle.GetComponent<RectTransform>().localScale = Vector3.zero;
                            scaleElapsedTime = 0f;
                            return true;
                        }
                    }
                    else
                    {
                        return false;
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

            /*if (Input.GetKeyDown(KeyCode.Space))
            {
                result = true;
            }*/

            if(true == isPlayerInput)
            {
                result = true;
                isPlayerInput = false;
            }

            return result;
        }

        private void RunZoomInOutEffect(float moveRatio)
        {
            float duration = moveRatio / jumpSpeed;
            moveTarget.GetComponent<ZoomInOut>().RunEffect(duration);
        }

        private float CircleEffect()
        {
            float curScale = 0f;

            float startScale = 1f;
            float endScale = 0.5f;
            float duration = 1f;
            if (scaleElapsedTime < 1)
            {
                scaleElapsedTime += Time.deltaTime;
                scaleElapsedTime = Mathf.Clamp01(scaleElapsedTime); // 0 ~ 1�� ����
            }
            else
            {
                scaleElapsedTime = 0f;
            }
            curScale = Mathf.Lerp(startScale, endScale, scaleElapsedTime / duration);
            inputTimingCircle.GetComponent<RectTransform>().localScale = new Vector3(curScale, curScale, curScale);

            return curScale;
        }
    }

}

