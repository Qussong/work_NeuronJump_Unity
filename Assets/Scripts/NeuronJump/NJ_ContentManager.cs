using System;
using System.Collections;
using System.Collections.Generic;
using GH;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


namespace GH
{
    public enum NJ_EContentState
    {
        Content1,   // Counting
        Content2,   // Explain
        Content3,   // JumpGame
        Content4,   // End
        None
    }

    public class NJ_ContentManager : ContentManager<NJ_ContentManager>
    {
        /// <summary>
        /// Counting Content 
        /// </summary>
        [Header("Content 1 : Counting")]
        [Tooltip("ī��Ʈ �ٿ��� ��µ� �̹��� ��ü")]
        [SerializeField] Image countdownDisplay = null;
        [Tooltip("ī��Ʈ �ٿ ����� �ؽ�ó ���ҽ���")]
        [SerializeField] List<Sprite> countdownImages = new List<Sprite>();
        [NonSerialized] private Coroutine countdownCoroutine;   // �������� �ڷ�ƾ ����

        [Header("Content 3")]
        [Tooltip("���� �÷����� ��ü ������ ����ϴ� ��ü")]
        [SerializeField] Scroller scroller;
        [Tooltip("���� �÷��� ���ѽð�")]
        [SerializeField] float gameTimer;
        [Tooltip("���ѽð� ������ ��ü")]
        [SerializeField] private TextMeshProUGUI gameTimerDisplay;
        [Tooltip("ī��Ʈ �ٿ��� ��µ� �̹��� ��ü")]
        [SerializeField] Image countdownDisplay2 = null;
        [Tooltip("ī��Ʈ �ٿ ����� �ؽ�ó ���ҽ���")]
        [SerializeField] List<Sprite> countdownImages2 = new List<Sprite>();
        [NonSerialized] private Coroutine countdownCoroutine2;   // �������� �ڷ�ƾ ����
        [NonSerialized] private Coroutine gameTimerCoroutine;   // �������� �ڷ�ƾ ����

        [Header("Content 4")]
        [Tooltip("���� ���� ����")]
        [SerializeField] private bool isGameEnd = false;
        [Tooltip("���� ���� ����")]
        [SerializeField] private bool isGameClear = false;
        [Tooltip("��� �̹��� ��� ��ü")]
        [SerializeField] Image resultDisplay;
        [Tooltip("�������ο� Ȱ��� �̹��� �����̳�")]
        [SerializeField] List<Sprite> resultImages = new List<Sprite>();

        public bool IsGameEnd
        {
            get { return isGameEnd; }
            set
            {
                isGameEnd = value;
                if (true == value)
                {
                    if (null != gameTimerCoroutine)
                    {
                        StopCoroutine(gameTimerCoroutine);
                        gameTimerCoroutine = null;

                        EndJumpGame();
                    }
                }
            }
        }

        public void Start()
        {
            if (0 == countdownImages.Count || null == countdownDisplay || 0 == countdownImages2.Count || null == countdownDisplay2)
                Debug.LogError("ī��Ʈ �ٿ ����� ��ü �Ǵ� �̹����� �������� �ʾҽ��ϴ�.");
            if (null == scroller) Debug.LogError("���� �÷����� ��ü ������ ����ϴ� ��ü�� �Ҵ���� �ʾҽ��ϴ�.");
            if (null == resultDisplay) Debug.LogError("���� �÷����� ����� ����� �̹��� ��ü�� �Ҵ���� �ʾҽ��ϴ�.");
        }

        protected override void CloseState(string state)
        {
            NJ_EContentState eState = (NJ_EContentState)Enum.Parse(typeof(NJ_EContentState), state);
            switch (eState)
            {
                case NJ_EContentState.Content1:
                    break;
                case NJ_EContentState.Content2:
                    break;
                case NJ_EContentState.Content3:
                    GetJumpGameResult();
                    break;
                default:
                    break;
            }
        }

        protected override void OpenState(string state)
        {
            NJ_EContentState eState = (NJ_EContentState)Enum.Parse(typeof(NJ_EContentState), state);
            switch (eState)
            {
                case NJ_EContentState.Content1:
                    // ī���� ���� �Լ� ȣ��
                    countdownCoroutine = RunCountdown(eState);
                    break;
                case NJ_EContentState.Content2:
                    DisplayTutorialImage();
                    break;
                case NJ_EContentState.Content3:
                    {
                        Debug.Log("Game Start!");
                        PlayJumpGame(eState);
                        break;
                    }
                case NJ_EContentState.Content4:
                    DisplayResultImage();
                    break;
                default:
                    break;
            }
        }

        private Coroutine RunCountdown(NJ_EContentState state)
        {
            // �������� �ڷ�ƾ�� �ִٸ� �����.
            if (null != countdownCoroutine) { StopCoroutine(countdownCoroutine); countdownCoroutine = null; }
            if (null != countdownCoroutine2) { StopCoroutine(countdownCoroutine2); countdownCoroutine2 = null; }
            // �� �ڷ�ƾ ����
            return StartCoroutine(CountdownCoroutine(state));
        }

        private IEnumerator CountdownCoroutine(NJ_EContentState state)
        {
            int count = countdownImages.Count;

            while (count > 0)
            {
                // �̹��� ���
                switch (state)
                {
                    case NJ_EContentState.Content1:
                        countdownDisplay.sprite = countdownImages[count - 1];
                        break;
                    case NJ_EContentState.Content3:
                        countdownDisplay2.sprite = countdownImages[count - 1];
                        break;
                    default:
                        break;
                }

                // ī��Ʈ �ٿ�
                count--;

                yield return new WaitForSeconds(1f);
            }

            if (0 == count)
            {
                switch (state)
                {
                    case NJ_EContentState.Content1:
                        {
                            // Explain ȭ������ �̵�
                            NJ_UIManager.Instance.MoveNextPage();  // Page : Counting -> Explain
                            break;
                        }
                    case NJ_EContentState.Content3:
                        {
                            if (null != gameTimerCoroutine) break;
                            // Countdown Display ����
                            countdownDisplay2.gameObject.SetActive(false);
                            // �������� ���� �÷��� ON
                            scroller.bPlay = true;
                            // ���� Ÿ�̸� ����
                            gameTimerCoroutine = StartCoroutine(GameTimer());
                            break;
                        }
                    default:
                        break;
                }
            }

        }

        private IEnumerator GameTimer()
        {
            float timer = gameTimer;

            while (timer > 0)
            {
                gameTimerDisplay.text = timer.ToString();

                // ī��Ʈ �ٿ�
                timer--;

                yield return new WaitForSeconds(1f);
            }
            gameTimerDisplay.text = "0";
            EndJumpGame();
        }

        private void DisplayTutorialImage()
        {
            //
        }

        private void PlayJumpGame(NJ_EContentState state)
        {
            countdownCoroutine2 = RunCountdown(state);
        }

        private void EndJumpGame()
        {
            scroller.bPlay = false;
            NJ_UIManager.Instance.MoveNextPage();   // Content3 (Game) -> Content4 (End)
        }

        private void GetJumpGameResult()
        {
            Debug.Log("Game End!");
            int remainTime = int.Parse(gameTimerDisplay.text);
            if (true == isGameEnd && remainTime > 0) isGameClear = true;
        }

        private void DisplayResultImage()
        {
            if (true == isGameClear) resultDisplay.sprite = resultImages[0];
            if (false == isGameClear) resultDisplay.sprite = resultImages[1];
        }
    }

}