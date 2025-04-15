using System;
using System.Collections;
using System.Collections.Generic;
using GH;
using UnityEngine;


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
        public void Start()
        {
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
