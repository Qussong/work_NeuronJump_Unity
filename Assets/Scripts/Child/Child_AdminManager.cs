using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GH
{
    public enum EAdminState
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

    /// <summary>
    /// 
    /// </summary>
    public class Child_AdminManager : AdminManager
    {
        public void Start()
        {
            string prevState = EAdminState.None.ToString();
            string newState = EAdminState.Init.ToString();
            StateMachine(new AdminStateChangedEventArgs(prevState, newState));
        }
        
        public void Update()
        {
        }

        protected override void CloseState(string state)
        {
            EAdminState eState = (EAdminState)Enum.Parse(typeof(EAdminState), state);
            switch (eState)
            {
                case EAdminState.Init:
                    break;
                case EAdminState.Running:
                    break;
                case EAdminState.Completed:
                    break;
                default:
                    break;
            }
        }

        protected override void OpenState(string state)
        {
            EAdminState eState = (EAdminState)Enum.Parse(typeof(EAdminState), state);
            switch (eState)
            {
                case EAdminState.Init:
                    InitProgram();
                    break;
                case EAdminState.Running:
                    break;
                case EAdminState.Completed:
                    CompletedProgram();
                    break;
                default:
                    break;
            }
        }

        public override void InitProgram()
        {
            //Debug.Log($"{this.GetType().Name}-Open-Init");
            base.InitProgram();
        }

        public override void CompletedProgram()
        {
            //Debug.Log($"{this.GetType().Name}-Open-Completed");
        }

    }
}
