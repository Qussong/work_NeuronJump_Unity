using System.Collections;
using System.Collections.Generic;
using GH;
using UnityEngine;


namespace GH
{
    public enum NJ_EUIPage
    {
        Home,
        Counting,
        Explain,
        JumpGame,
        End,
        None,
    }

    public class NJ_UIManager : UIManager<NJ_UIManager>
    {
        protected override void Awake()
        {
            base.Awake();

            // 시리얼 통신 Delegate 함수 등록
            NetworkManager.Instance.reciveData += OnDPacketDelegate_UI;
        }

        public override void OnPageChanged(object sender, PageChangedEventArgs e)
        {
            base.OnPageChanged(sender, e);

            // Admin 
            string adminPrevState = GetAdminState((NJ_EUIPage)e.PrevPageIdx);
            string adminNewState = GetAdminState((NJ_EUIPage)e.NewPageIdx);
            NJ_AdminManager.Instance.StateMachine(new AdminStateChangedEventArgs(adminPrevState, adminNewState));

            // Content
            string contentPrevState = GetContentState((NJ_EUIPage)e.PrevPageIdx);
            string contentNewState = GetContentState((NJ_EUIPage)e.NewPageIdx);
            NJ_ContentManager.Instance.StateMachine(new ContentStateChangedEventArgs(contentPrevState, contentNewState));
        }

        // UI 페이지에 대응하는 AdminState 반환
        private string GetAdminState(NJ_EUIPage page)
        {
            return page switch
            {
                NJ_EUIPage.Home => NJ_EAdminState.Init.ToString(),
                NJ_EUIPage.Counting => NJ_EAdminState.Running.ToString(),
                NJ_EUIPage.End => NJ_EAdminState.Completed.ToString(),
                _ => NJ_EAdminState.None.ToString(),
            };
        }

        private string GetContentState(NJ_EUIPage page)
        {
            return page switch
            {
                NJ_EUIPage.Counting => NJ_EContentState.Content1.ToString(),
                NJ_EUIPage.Explain => NJ_EContentState.Content2.ToString(),
                NJ_EUIPage.JumpGame => NJ_EContentState.Content3.ToString(),
                NJ_EUIPage.End => NJ_EContentState.Content4.ToString(),
                _ => NJ_EContentState.None.ToString()
            };
        }

        public void OnDPacketDelegate_UI(byte[] bytes)
        {
            if(NJ_EUIPage.Home == (NJ_EUIPage)CurPageIdx)
            {
                MoveNextPage();
            }

            if (NJ_EUIPage.Explain == (NJ_EUIPage)CurPageIdx)
            {
                MoveNextPage();
            }

            if (NJ_EUIPage.End == (NJ_EUIPage)CurPageIdx)
            {
                MoveHome();
            }
        }

    }

}

