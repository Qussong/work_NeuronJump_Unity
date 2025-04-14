using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

namespace GH
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUIManager
    {
        // �̺�Ʈ �ڵ鷯 ����
        event EventHandler MoveHomeHandler;
        event EventHandler MovePrevPageHandler;
        event EventHandler MoveNextPageHandler;
        event EventHandler<PageChangedEventArgs> PageChangedHandler;

        // ������ �̵� �޼���
        void MoveHome();
        void MovePrevPage();
        void MoveNextPage();
        void PageChanged(PageChangedEventArgs e);

        // ������ �̵� �ڵ鷯 �޼���
        void OnMoveHome(object sender, EventArgs e);
        void OnMovePrevPage(object sender, EventArgs e);
        void OnMoveNextPage(object sender, EventArgs e);
        void OnPageChanged(object sender, PageChangedEventArgs e);

        // UI ���� ����
        bool HasUIElement<T>(T enumId) where T : Enum;

        //  UI Ȱ��ȭ/��Ȱ��ȭ
        void ActivateUI<T>(T enumId) where T : Enum;
        void DeactivateUI<T>(T enumId) where T : Enum;
    }

    /// <summary>
    /// ������ ���� �̺�Ʈ ������
    /// </summary>
    public class PageChangedEventArgs : EventArgs
    {
        public PageChangedEventArgs(int prevPageIdx, int newPageIdx) 
        {
            PrevPageIdx = prevPageIdx;
            NewPageIdx = newPageIdx;
        }
        public int PrevPageIdx { get; set; }
        public int NewPageIdx { get; set; }
    }

}
