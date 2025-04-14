using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GH
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAdminManager
    {
        // �̺�Ʈ �ڵ鷯 ����
        event EventHandler<AdminStateChangedEventArgs> AdminStateChangeHandler;
        // ���α׷� �÷ο� ���� �޼���
        void StateMachine(AdminStateChangedEventArgs e);
        // �̺�Ʈ �ڵ鷯 �޼���
        void OnStateMachine(object sender, AdminStateChangedEventArgs e);
        // ���α׷� �ʱ�ȭ/����
        void InitProgram();
        // ���α׷� �Ϸ�/����
        void CompletedProgram();
    }

    public class AdminStateChangedEventArgs : EventArgs
    {
        public AdminStateChangedEventArgs(string prevState, string newState)
        {
            PrevState = prevState;
            NewState = newState;
        }
        public string PrevState { get; set; }
        public string NewState { get; set; }
    }

}
