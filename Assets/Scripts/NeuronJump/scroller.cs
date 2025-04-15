using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


namespace GH
{
    public class Scroller : MonoBehaviour
    {
        [Tooltip("��ü ��� ���� ������ ��ü")]
        [SerializeField] private GameObject moveTarget;
        [Tooltip("��ũ�� ���� Scrool Rect ������Ʈ")]
        [SerializeField] private ScrollRect scrollRect;
        [Tooltip("���� ����")]
        [SerializeField] GameObject startPoint;
        [Tooltip("�� ����")]
        [SerializeField] GameObject endPoint;
        [Tooltip("��θ� �̷�� Bezier Curve ���� �����̳�")]
        [SerializeField] List<BezierCurve> curves = new List<BezierCurve>();
        [Tooltip("��θ� �̷�� Beizer ��� ���� ��ġ�� ��� Ȱ��ȭ�� ���� �ؽ� ���̺�")]
        [SerializeField] List<bool> actives = new List<bool>();
        [Tooltip("��θ� �̷�� Beizer ��� ���� ��ġ�� ��� �ε���")]
        [SerializeField] int curLevel = 0;
        //[Tooltip("���� �Ϸ� ����")]
        //[SerializeField] bool bFinish = false;
        private float totalLength = 0f; // ��ü ����� ���۰� ������ Y��

        // Game
        [Tooltip("���� ���� �÷���")]
        [SerializeField] public bool bPlay = false; 
        [SerializeField] GameObject Ring;
        [Tooltip("���� ���� ���� ����")]
        [SerializeField] List<bool> jumpRoutes = new List<bool>();
        [Tooltip("�ش� �������� �����ؾ��� ���� �Ϸ� ����")]
        [SerializeField] bool bFinishJump = false;
        [Tooltip("���� ������ ���� ���ߴ� ��ġ�� �����ϴ� �����̳�")]
        [SerializeField] List<float> stopPosRatios = new List<float>();
        [Tooltip("���� ���� ���� �ε���")]
        [SerializeField] int jumpNeuronIdx = 0;
        [Tooltip("���� ���� �ε���")]
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

                // �÷��̾��� �Է��� �䱸�Ǵ� ��Ȳ
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

            // ���� ���� ������ ��� õõ�� �̵�
            if (true == jumpRoutes[curLevel])
                curves[curLevel].progress += Time.deltaTime / 5f;
            // ���� ���� ������ �ƴѰ�� ���� �̵�
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

