using System;
using System.Collections;
using System.Collections.Generic;
using GH;
using UnityEngine;
using UnityEngine.UI;


namespace GH
{
    public enum NJ_EAdminState
    {
        Init,       // 초기화 중 (=Initializing)
        Running,    // 실행 중
        Completed,  // 완료 됨
        Idle,       // 대기 상태
        Error,      // 오류 발생
        Paused,     // 일시 정지
        Restarting, // 재시작
        None,
    }

    public class NJ_AdminManager : AdminManager<NJ_AdminManager>
    {
        #region Program Control

        [Header("Program Control")]
        [Tooltip("프로그램 종료 버튼")]
        [SerializeField] private Button exitButton;
        [Tooltip("프로그램 종료 함수 호출 조건")]
        [ReadOnly][SerializeField] private int exitCnt;
        Coroutine exitCntResetCoroutine;
        [Tooltip("관람객의 프로그램 사용 여부 확인")]
        [ReadOnly][SerializeField] private bool bIdle;

        public bool BIdle
        {
            get { return bIdle; }
            set { bIdle = value; }
        }

        private void Init_ProgramControl()
        {
            if (null == exitButton) Debug.LogError("종료 버튼이 설정되지 않았습니다.");
            exitButton.onClick.AddListener(OnExitButton);
            exitCnt = 0;
            bIdle = true;

            // 대기상태시 홈화면으로 되돌아가는 코루틴 함수 실행
            StartCoroutine(OnReturnToHome());
        }

        private void OnExitButton()
        {
            //Debug.LogWarning("프로그램 종료 함수가 호출되었습니다.");
            ++exitCnt;

            // 종료 카운트(exitCnt) 초기화 코루틴 함수 호출
            if (null == exitCntResetCoroutine)
            {
                exitCntResetCoroutine = StartCoroutine(OnExitCntResetCoroutine());
            }

            // 종료 조건 충족시 프로그램 종료
            if (exitCnt > 10)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode(); // 에디터에서 실행 시 종료
#else
                Application.Quit(); // 빌드된 프로그램에서 실행 시 종료
#endif
            }
        }

        private IEnumerator OnExitCntResetCoroutine()
        {
            yield return new WaitForSeconds(10f);
            exitCntResetCoroutine = null;
            exitCnt = 0;
        }

        private IEnumerator OnReturnToHome()
        {
            // 대기상태 체크 코루틴함수 호출
            StartCoroutine(OnCheckIdle());

            // term 간격마다 Home 화면으로 돌아가는 함수 호출
            float term = 180f;  // 3 min
            float elapsedTime = 0f;
            while (true)
            {
                if (false == bIdle)
                {
                    elapsedTime = 0f;
                    bIdle = true;
                }
                elapsedTime += Time.deltaTime;
                if (elapsedTime > term)
                {
                    NJ_UIManager.Instance.MoveHome();
                }
                yield return null;
            }
        }

        private IEnumerator OnCheckIdle()
        {
            while (true)
            {
                if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                {
                    Debug.Log("화면이 터치되었습니다!");
                    bIdle = false;
                }
                yield return null;
            }
        }

        #endregion

        public void Start()
        {
            // Program Control
            Init_ProgramControl();

            string prevState = NJ_EAdminState.None.ToString();
            string newState = NJ_EAdminState.Init.ToString();
            StateMachine(new AdminStateChangedEventArgs(prevState, newState));
        }

        protected override void CloseState(string state)
        {
            NJ_EAdminState eState = (NJ_EAdminState)Enum.Parse(typeof(NJ_EAdminState), state);
            switch (eState)
            {
                case NJ_EAdminState.Init:
                    break;
                case NJ_EAdminState.Running:
                    break;
                case NJ_EAdminState.Completed:
                    break;
                default:
                    break;
            }
        }

        protected override void OpenState(string state)
        {
            NJ_EAdminState eState = (NJ_EAdminState)Enum.Parse(typeof(NJ_EAdminState), state);
            switch (eState)
            {
                case NJ_EAdminState.Init:
                    InitProgram();
                    break;
                case NJ_EAdminState.Running:
                    break;
                case NJ_EAdminState.Completed:
                    CompletedProgram();
                    break;
                default:
                    break;
            }
        }

        public override void InitProgram()
        {
            base.InitProgram();
            NJ_UIManager.Instance.InitUI();
        }

        public override void CompletedProgram()
        {
            //
        }
    }


}
