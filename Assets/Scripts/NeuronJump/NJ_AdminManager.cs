using System;
using System.Collections;
using System.Collections.Generic;
using GH;
using UnityEngine;

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

public class NJ_AdminManager : AdminManager
{
    public void Start()
    {
        string prevState = NJ_EAdminState.None.ToString();
        string newState = NJ_EAdminState.Init.ToString();
        StateMachine(new AdminStateChangedEventArgs(prevState, newState));
    }

    public void Update()
    {
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
        //Debug.Log($"{this.GetType().Name}-Open-Init");
        base.InitProgram();
    }

    public override void CompletedProgram()
    {
        //Debug.Log($"{this.GetType().Name}-Open-Completed");
    }
}
