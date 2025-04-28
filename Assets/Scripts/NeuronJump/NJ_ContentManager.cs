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
        [Tooltip("ī��Ʈ �ٿ��� ��µ� �̹��� ��ü")]
        [SerializeField] Image countdownDisplay = null;
        [Tooltip("ī��Ʈ �ٿ ����� �ؽ�ó ���ҽ���")]
        [SerializeField] List<Sprite> countdownImages = new List<Sprite>();
        [NonSerialized] private Coroutine countdownCoroutine;   // �������� �ڷ�ƾ ����

        private void Init_Content1()
        {
            Debug.Log("Init Content 1");

            if (null == countdownDisplay) Debug.LogError("ī��Ʈ �ٿ ����� ��ü �Ǵ� �̹����� �������� �ʾҽ��ϴ�.");
            if (0 == countdownImages.Count) Debug.LogError("ī��Ʈ �ٿ ����� �̹����� �������� �ʾҽ��ϴ�.");
            if(null != countdownCoroutine) { StopCoroutine(countdownCoroutine); countdownCoroutine = null; }
        }

        private void Init_Content2()
        {
            Debug.Log("Init Content 2");
        }

        [Header("Content 3 : Jump Game")]
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

        private void Init_Content3()
        {
            Debug.Log("Init Content 3");

            if (null == scroller) Debug.LogError("���� �÷����� ��ü ������ ����ϴ� ��ü�� �Ҵ���� �ʾҽ��ϴ�.");
            if (null == gameTimerDisplay) Debug.LogError("���� ���� �ð��� ����� Ÿ�̸� ���÷��� ��ü�� �Ҵ���� �ʾҽ��ϴ�.");
            if (0 == countdownImages2.Count) Debug.LogError("ī��Ʈ �ٿ ����� �̹����� �������� �ʾҽ��ϴ�.");
            if (null != countdownCoroutine2) { StopCoroutine(countdownCoroutine2); countdownCoroutine2 = null; }
            if (null != gameTimerCoroutine) { StopCoroutine(gameTimerCoroutine); gameTimerCoroutine = null; }

            gameTimer = 20; // 20 sec
        }

        [Header("Content 4 : End")]
        [Tooltip("���� ���� ����")]
        [ReadOnly][SerializeField] private bool isGameEnd;
        [Tooltip("���� ���� ����")]
        [ReadOnly][SerializeField] private bool isGameClear;
        [Tooltip("��� �̹��� ��� ��ü")]
        [SerializeField] Image BGDisplay;
        [Tooltip("�������ο� Ȱ��� ��� �̹��� �����̳�")]
        [SerializeField] List<Sprite> BGImages = new List<Sprite>();
        [Tooltip("���� ��� �̹��� ��� ��ü")]
        [SerializeField] Image nueoDisplay;
        [Tooltip("�������ο� Ȱ��� ���� �̹��� �����̳�")]
        [SerializeField] List<Sprite> neuoImages = new List<Sprite>();
        [Tooltip("�������θ� ����� Ÿ��Ʋ ���÷��� ��ü �����̳�")]
        [SerializeField] List<TextMeshProUGUI> titleDisplays = new List<TextMeshProUGUI>();
        [Tooltip("�������ο� Ȱ��� Ÿ��Ʋ ���� �����̳�")]
        [SerializeField] List<string> titleTexts = new List<string>();
        [Tooltip("�������ο� Ȱ��� Ÿ��Ʋ ���� �����̳�")]
        [SerializeField] List<Color> titleColor = new List<Color>();

        private void Init_Content4()
        {
            Debug.Log("Init Content 4");

            if (null == BGDisplay) Debug.LogError("���� �÷��� ��� ȭ���� ����̹����� ����� ���÷��� ��ü�� �Ҵ���� �ʾҽ��ϴ�.");
            if (0 == BGImages.Count) Debug.LogError("��� �̹����� �������� �ʾҽ��ϴ�.");
            if (null == nueoDisplay) Debug.LogError("���� �̹����� ����� ���÷��� ��ü�� �Ҵ���� �ʾҽ��ϴ�.");
            if (0 == neuoImages.Count) Debug.LogError("���� �̹����� �������� �ʾҽ��ϴ�.");
            if (0 == titleDisplays.Count) Debug.LogError("�������θ� ����� Ÿ��Ʋ ���÷��� ��ü�� �������� �ʾҽ��ϴ�. (2��)");
            if (0 == titleTexts.Count) Debug.LogError("�������ο� Ȱ��� Ÿ��Ʋ ������ �������� �ʾҽ��ϴ�.");
            if (0 == titleColor.Count) Debug.LogError("�������ο� Ȱ��� Ÿ��Ʋ ������ �������� �ʾҽ��ϴ�.");

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

            // �ø��� ��� Delegate �Լ� ���
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
                nueoDisplay.sprite = neuoImages[0];     // ���� �̹���
                BGDisplay.sprite = BGImages[0];         // ��� �̹���
                titleDisplays[0].text = titleTexts[0];  // Ÿ��Ʋ �ؽ�Ʈ 1 ����
                titleDisplays[0].color = titleColor[0]; // Ÿ��Ʋ �ؽ�Ʈ 1 ����
                titleDisplays[1].text = titleTexts[1];  // Ÿ��Ʋ �ؽ�Ʈ 2 ����
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

            // ���� ���� �������� Ȯ�� (���� : �����ð��� 0���� ŭ)
            String remainTimeTxt = gameTimerDisplay.text;   // �����ð� ��������
            int remainTime = int.Parse(remainTimeTxt.Substring(0, remainTimeTxt.Length - 1));   // string -> int ��ȯ
            if (true == isGameEnd && remainTime > 0) isGameClear = true;    // �����ð��� 0���� ũ�� �������� ����
            // ���� �ð� ��� �ʱ�ȭ
            gameTimerDisplay.text = "0��";
            // ī��Ʈ �ٿ� �ʱ�ȭ
            countdownDisplay2.gameObject.SetActive(true);
            // ���� ���� ���� �ʱ�ȭ
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
                // �̹��� ���
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
                gameTimerDisplay.text = timer.ToString() + "��";

                // ī��Ʈ �ٿ�
                timer--;

                yield return new WaitForSeconds(1f);
            }
            gameTimerDisplay.text = "0" + "��";
            NJ_UIManager.Instance.MoveNextPage();   // Content3 (Game) -> Content4 (End)
        }

        public void OnDPacketDelegate_Content(byte[] bytes)
        {
            if (NJ_EUIPage.JumpGame != (NJ_EUIPage)NJ_UIManager.Instance.CurPageIdx) return;

            scroller.IsPlayerInput = true;
        }

    }
}