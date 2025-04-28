using System;
using System.Collections;
using System.Collections.Generic;
using GH;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;


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
        [Header("Content 1 : Counting")]
        [Tooltip("카운트 다운이 출력될 이미지 객체")]
        [SerializeField] Image countdownDisplay = null;
        [Tooltip("카운트 다운에 사용할 텍스처 리소스들")]
        [SerializeField] List<Sprite> countdownImages = new List<Sprite>();
        [NonSerialized] private Coroutine countdownCoroutine;   // 실행중인 코루틴 참조

        private void Init_Content1()
        {
            Debug.Log("Init Content 1");

            if (null == countdownDisplay) Debug.LogError("카운트 다운에 사용할 객체 또는 이미지가 설정되지 않았습니다.");
            if (0 == countdownImages.Count) Debug.LogError("카운트 다운에 사용할 이미지가 설정되지 않았습니다.");
            if(null != countdownCoroutine) { StopCoroutine(countdownCoroutine); countdownCoroutine = null; }
        }

        private void Init_Content2()
        {
            Debug.Log("Init Content 2");
        }

        [Header("Content 3 : Jump Game")]
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

        private void Init_Content3()
        {
            Debug.Log("Init Content 3");

            if (null == scroller) Debug.LogError("게임 플레이의 전체 로직을 담당하는 객체가 할당되지 않았습니다.");
            if (null == gameTimerDisplay) Debug.LogError("게임 제한 시간을 출력할 타이머 디스플레이 객체가 할당되지 않았습니다.");
            if (0 == countdownImages2.Count) Debug.LogError("카운트 다운에 사용할 이미지가 설정되지 않았습니다.");
            if (null != countdownCoroutine2) { StopCoroutine(countdownCoroutine2); countdownCoroutine2 = null; }
            if (null != gameTimerCoroutine) { StopCoroutine(gameTimerCoroutine); gameTimerCoroutine = null; }

            gameTimer = 20; // 20 sec
        }

        [Header("Content 4 : End")]
        [Tooltip("게임 종료 여부")]
        [ReadOnly][SerializeField] private bool isGameEnd;
        [Tooltip("게임 성공 여부")]
        [ReadOnly][SerializeField] private bool isGameClear;
        [Tooltip("결과 이미지 출력 객체")]
        [SerializeField] Image BGDisplay;
        [Tooltip("성공여부에 활용될 배경 이미지 컨테이너")]
        [SerializeField] List<Sprite> BGImages = new List<Sprite>();
        [Tooltip("뉴오 결과 이미지 출력 객체")]
        [SerializeField] Image nueoDisplay;
        [Tooltip("성공여부에 활용될 뉴오 이미지 컨테이너")]
        [SerializeField] List<Sprite> neuoImages = new List<Sprite>();
        [Tooltip("성공여부를 출력할 타이틀 디스플레이 객체 컨테이너")]
        [SerializeField] List<TextMeshProUGUI> titleDisplays = new List<TextMeshProUGUI>();
        [Tooltip("성공여부에 활용될 타이틀 문구 컨테이너")]
        [SerializeField] List<string> titleTexts = new List<string>();
        [Tooltip("성공여부에 활용될 타이틀 색상 컨테이너")]
        [SerializeField] List<Color> titleColor = new List<Color>();

        private void Init_Content4()
        {
            Debug.Log("Init Content 4");

            if (null == BGDisplay) Debug.LogError("게임 플레이 결과 화면의 배경이미지를 출력할 디스플레이 객체가 할당되지 않았습니다.");
            if (0 == BGImages.Count) Debug.LogError("배경 이미지가 설정되지 않았습니다.");
            if (null == nueoDisplay) Debug.LogError("뉴오 이미지를 출력할 디스플레이 객체가 할당되지 않았습니다.");
            if (0 == neuoImages.Count) Debug.LogError("뉴오 이미지가 설정되지 않았습니다.");
            if (0 == titleDisplays.Count) Debug.LogError("성공여부를 출력할 타이틀 디스플레이 객체가 설정되지 않았습니다. (2개)");
            if (0 == titleTexts.Count) Debug.LogError("성공여부에 활용될 타이틀 문구가 설정되지 않았습니다.");
            if (0 == titleColor.Count) Debug.LogError("성공여부에 활용될 타이틀 색상이 설정되지 않았습니다.");

            isGameEnd = false;
            isGameClear = false;
        }

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
                        NJ_UIManager.Instance.MoveNextPage();   // Content3 (Game) -> Content4 (End)
                    }
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            // 시리얼 통신 Delegate 함수 등록
            NetworkManager.Instance.reciveData += OnDPacketDelegate_Content;
        }

        public void Start()
        {
            Init_Content1();
            Init_Content2();
            Init_Content3();
            Init_Content4();
        }

        protected override void OpenState(string state)
        {
            NJ_EContentState eState = (NJ_EContentState)Enum.Parse(typeof(NJ_EContentState), state);
            switch (eState)
            {
                case NJ_EContentState.Content1:
                    {
                        Start_Content1(eState);
                        break;
                    }
                case NJ_EContentState.Content2:
                    {
                        Start_Content2(eState);
                        break;
                    }
                case NJ_EContentState.Content3:
                    {
                        Start_Content3(eState);
                        break;
                    }
                case NJ_EContentState.Content4:
                    {           
                        Start_Content4(eState);
                        break;
                    }
                default:
                    break;
            }
        }

        protected override void CloseState(string state)
        {
            NJ_EContentState eState = (NJ_EContentState)Enum.Parse(typeof(NJ_EContentState), state);
            switch (eState)
            {
                case NJ_EContentState.Content1:
                    End_Content1(eState);
                    break;
                case NJ_EContentState.Content2:
                    End_Content2(eState);
                    break;
                case NJ_EContentState.Content3:
                    End_Content3(eState);
                    break;
                case NJ_EContentState.Content4:
                    End_Content4(eState);
                    break;
                default:
                    break;
            }
        }

        private void Start_Content1(NJ_EContentState e)
        {
            countdownCoroutine = StartCoroutine(CountdownCoroutine(e));
        }

        private void Start_Content2(NJ_EContentState e)
        {
            //
        }

        private void Start_Content3(NJ_EContentState e)
        {
            countdownCoroutine2 = StartCoroutine(CountdownCoroutine(e));
        }

        private void Start_Content4(NJ_EContentState e)
        {
            if (true == isGameClear)
            {
                nueoDisplay.sprite = neuoImages[0];     // 뉴오 이미지
                BGDisplay.sprite = BGImages[0];         // 배경 이미지
                titleDisplays[0].text = titleTexts[0];  // 타이틀 텍스트 1 문구
                titleDisplays[0].color = titleColor[0]; // 타이틀 텍스트 1 색상
                titleDisplays[1].text = titleTexts[1];  // 타이틀 텍스트 2 문구
            }

            if (false == isGameClear)
            {
                nueoDisplay.sprite = neuoImages[1];
                BGDisplay.sprite = BGImages[1];
                titleDisplays[0].text = titleTexts[2];
                titleDisplays[0].color = titleColor[1];
                titleDisplays[1].text = titleTexts[3];
            }

            nueoDisplay.SetNativeSize();
        }

        private void End_Content1(NJ_EContentState e)
        {
            Init_Content1();
        }

        private void End_Content2(NJ_EContentState e)
        {
            Init_Content2();
        }

        private void End_Content3(NJ_EContentState e)
        {
            Init_Content3();

            // 점프 게임 성공조건 확인 (조건 : 남은시간이 0보다 큼)
            String remainTimeTxt = gameTimerDisplay.text;   // 남은시간 가져오기
            int remainTime = int.Parse(remainTimeTxt.Substring(0, remainTimeTxt.Length - 1));   // string -> int 변환
            if (true == isGameEnd && remainTime > 0) isGameClear = true;    // 남은시간이 0보다 크면 성공조건 만족
            // 남은 시간 출력 초기화
            gameTimerDisplay.text = "0초";
            // 카운트 다운 초기화
            countdownDisplay2.gameObject.SetActive(true);
            // 점프 게임 로직 초기화
            scroller.Init();
        }

        private void End_Content4(NJ_EContentState e)
        {
            Init_Content4();
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
                        countdownDisplay.SetNativeSize();
                        break;
                    case NJ_EContentState.Content3:
                        countdownDisplay2.sprite = countdownImages[count - 1];
                        countdownDisplay2.SetNativeSize();
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
                gameTimerDisplay.text = timer.ToString() + "초";

                // 카운트 다운
                timer--;

                yield return new WaitForSeconds(1f);
            }
            gameTimerDisplay.text = "0" + "초";
            NJ_UIManager.Instance.MoveNextPage();   // Content3 (Game) -> Content4 (End)
        }

        public void OnDPacketDelegate_Content(byte[] bytes)
        {
            if (NJ_EUIPage.JumpGame != (NJ_EUIPage)NJ_UIManager.Instance.CurPageIdx) return;

            scroller.IsPlayerInput = true;
        }

    }
}