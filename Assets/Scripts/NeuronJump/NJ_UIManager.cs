using System.Collections;
using System.Collections.Generic;
using GH;
using UnityEngine;

public enum NJ_EUIPage
{
    Home,
    Counting,
    Explain,
    JumpGame,
    End,
    None,
}

public class NJ_UIManager : UIManager
{
    public override void OnPageChanged(object sender, PageChangedEventArgs e)
    {
        base.OnPageChanged(sender, e);

        // Admin 
        string adminPrevState = GetAdminState((NJ_EUIPage)e.PrevPageIdx);
        string adminNewState = GetAdminState((NJ_EUIPage)e.NewPageIdx);
        Child_AdminManager.Instance.StateMachine(new AdminStateChangedEventArgs(adminPrevState, adminNewState));

        // Content
        string contentPrevState = GetContentState((NJ_EUIPage)e.PrevPageIdx);
        string contentNewState = GetContentState((NJ_EUIPage)e.NewPageIdx);
        Child_ContentManager.Instance.StateMachine(new ContentStateChangedEventArgs(contentPrevState, contentNewState));
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
            _ => NJ_EContentState.None.ToString()
        };
    }
}
