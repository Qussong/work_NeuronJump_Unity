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
        [Tooltip("카운트 다운이 출력될 이미지 객체")]
        [SerializeField] Image countdownDisplay = null;
        [Tooltip("카운트 다운에 사용할 텍스처 리소스들")]
        [SerializeField] List<Sprite> countdownImages = new List<Sprite>();
        [NonSerialized] private Coroutine countdownCoroutine;   // 실행중인 코루틴 참조

        [Header("Content 3")]
        [Tooltip("게임 플레이의 전체 로직을 담당하는 객체")]
        [SerializeField] Scroller scroller;
        [Tooltip("게임 플레이 제한시간")]
        [SerializeField] float gameTimer;
        [Tooltip("제한시간 보여줄 객체")]
        [SerializeField] private TextMeshProUGUI gameTimerDisplay;
        [Tooltip("카운트 다운이 출력될 이미지 객체")]
        [SerializeField] Image countdownDisplay2 = null;
        [Tooltip("카운트 다운에 사용할 텍스처 리소스들")]
        [SerializeField] List<Sprite> countdownImages2 = new List<Sprite>();
        [NonSerialized] private Coroutine countdownCoroutine2;   // 실행중인 코루틴 참조
        [NonSerialized] private Coroutine gameTimerCoroutine;   // 실행중인 코루틴 참조

        [Header("Content 4")]
        [Tooltip("게임 종료 여부")]
        [SerializeField] private bool isGameEnd = false;
        [Tooltip("게임 성공 여부")]
        [SerializeField] private bool isGameClear = false;
        [Tooltip("결과 이미지 출력 객체")]
        [SerializeField] Image resultDisplay;
        [Tooltip("성공여부에 활용될 이미지 컨테이너")]
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
                Debug.LogError("카운트 다운에 사용할 객체 또는 이미지가 설정되지 않았습니다.");
            if (null == scroller) Debug.LogError("게임 플레이의 전체 로직을 담당하는 객체가 할당되지 않았습니다.");
            if (null == resultDisplay) Debug.LogError("게임 플레이의 결과를 출력할 이미지 객체가 할당되지 않았습니다.");
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
                    // 카운팅 관련 함수 호출
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
            // 실행중인 코루틴이 있다면 멈춘다.
            if (null != countdownCoroutine) { StopCoroutine(countdownCoroutine); countdownCoroutine = null; }
            if (null != countdownCoroutine2) { StopCoroutine(countdownCoroutine2); countdownCoroutine2 = null; }
            // 새 코루틴 시작
            return StartCoroutine(CountdownCoroutine(state));
        }

        private IEnumerator CountdownCoroutine(NJ_EContentState state)
        {
            int count = countdownImages.Count;

            while (count > 0)
            {
                // 이미지 출력
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

                // 카운트 다운
                count--;

                yield return new WaitForSeconds(1f);
            }

            if (0 == count)
            {
                switch (state)
                {
                    case NJ_EContentState.Content1:
                        {
                            // Explain 화면으로 이동
                            NJ_UIManager.Instance.MoveNextPage();  // Page : Counting -> Explain
                            break;
                        }
                    case NJ_EContentState.Content3:
                        {
                            if (null != gameTimerCoroutine) break;
                            // Countdown Display 숨김
                            countdownDisplay2.gameObject.SetActive(false);
                            // 점프게임 시작 플래그 ON
                            scroller.bPlay = true;
                            // 게임 타이머 시작
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

                // 카운트 다운
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