using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace GH
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContentManager
    {
        // �̺�Ʈ �ڵ鷯 ����
        event EventHandler<ContentStateChangedEventArgs> ContentStateChangeHandler;

        // ������ ���� �޼���
        void StateMachine(ContentStateChangedEventArgs e);

        // ���� �ӽ� �ڵ鷯 �޼���
        void OnStateMachine(object sender, ContentStateChangedEventArgs e);
    }

    public class ContentStateChangedEventArgs : EventArgs
    {
        public ContentStateChangedEventArgs(string prevState, string newState) 
        {
            PrevState = prevState;
            NewState = newState;
        }
        public string PrevState { get; set; }
        public string NewState { get; set; }
    }

}
