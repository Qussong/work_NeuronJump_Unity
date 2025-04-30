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
        Init,       // �ʱ�ȭ �� (=Initializing)
        Running,    // ���� ��
        Completed,  // �Ϸ� ��
        Idle,       // ��� ����
        Error,      // ���� �߻�
        Paused,     // �Ͻ� ����
        Restarting, // �����
        None,
    }

    public class NJ_AdminManager : AdminManager<NJ_AdminManager>
    {
        #region Program Control

        [Header("Program Control")]
        [Tooltip("���α׷� ���� ��ư")]
        [SerializeField] private Button exitButton;
        [Tooltip("���α׷� ���� �Լ� ȣ�� ����")]
        [ReadOnly][SerializeField] private int exitCnt;
        Coroutine exitCntResetCoroutine;
        [Tooltip("�������� ���α׷� ��� ���� Ȯ��")]
        [ReadOnly][SerializeField] private bool bIdle;

        public bool BIdle
        {
            get { return bIdle; }
            set { bIdle = value; }
        }

        private void Init_ProgramControl()
        {
            if (null == exitButton) Debug.LogError("���� ��ư�� �������� �ʾҽ��ϴ�.");
            exitButton.onClick.AddListener(OnExitButton);
            exitCnt = 0;
            bIdle = true;

            // �����½� Ȩȭ������ �ǵ��ư��� �ڷ�ƾ �Լ� ����
            StartCoroutine(OnReturnToHome());
        }

        private void OnExitButton()
        {
            //Debug.LogWarning("���α׷� ���� �Լ��� ȣ��Ǿ����ϴ�.");
            ++exitCnt;

            // ���� ī��Ʈ(exitCnt) �ʱ�ȭ �ڷ�ƾ �Լ� ȣ��
            if (null == exitCntResetCoroutine)
            {
                exitCntResetCoroutine = StartCoroutine(OnExitCntResetCoroutine());
            }

            // ���� ���� ������ ���α׷� ����
            if (exitCnt > 10)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode(); // �����Ϳ��� ���� �� ����
#else
                Application.Quit(); // ����� ���α׷����� ���� �� ����
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
            // ������ üũ �ڷ�ƾ�Լ� ȣ��
            StartCoroutine(OnCheckIdle());

            // term ���ݸ��� Home ȭ������ ���ư��� �Լ� ȣ��
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
                    Debug.Log("ȭ���� ��ġ�Ǿ����ϴ�!");
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
