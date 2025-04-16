using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

namespace GH
{
    public enum EUIPage
    {
        Home,
        Content,
        End,
        None,
    }

    /// <summary>
    /// 
    /// </summary>
    public class Child_UIManager : UIManager<Child_UIManager>
    {
        public override void OnPageChanged(object sender, PageChangedEventArgs e)
        {
            base.OnPageChanged(sender, e);

            string adminPrevState = GetAdminState((EUIPage)e.PrevPageIdx);
            string adminNewState = GetAdminState((EUIPage)e.NewPageIdx);
            Child_AdminManager.Instance.StateMachine(new AdminStateChangedEventArgs(adminPrevState, adminNewState));
        }

        // UI 페이지에 대응하는 AdminState 반환
        private string GetAdminState(EUIPage page)
        {
            return page switch
            {
                EUIPage.Home => EAdminState.Init.ToString(),
                EUIPage.Content => EAdminState.Running.ToString(),
                EUIPage.End => EAdminState.Completed.ToString(),
                _ => EAdminState.None.ToString(),
            };
        }

    }
}
