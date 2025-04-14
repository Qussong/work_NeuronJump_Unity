using System;
using System.Collections;
using System.Collections.Generic;
using GH;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum NJ_EContentState
{
    Content1,   // Counting
    Content2,   // Explain
    Content3,   // JumpGame
    None
}

public class NJ_ContentManager : ContentManager
{
    /// <summary>
    /// Counting Content 
    /// </summary>
    [Header("Content 1 : Counting")]
    [Tooltip("카운트 다운이 출력될 이미지 객체")]
    [SerializeField] Image countdownDisplay = null;
    [Tooltip("카운트 다운에 사용할 텍스처 리소스들")]
    [SerializeField] List<Sprite> countdownImages = new List<Sprite>();
    [NonSerialized] private Coroutine countdownCoroutine;   // 실행중인 코루틴 참조

    [Header("Content 3")]
    [Tooltip("게임 클리어 여부")]
    [SerializeField][ReadOnly] bool isClear = true;

    public void Start()
    {
        if (0 == countdownImages.Count || null == countdownDisplay)
        {
            Debug.LogError("카운트 다운에 사용할 객체 또는 이미지가 설정되지 않았습니다.");
        }
    }

    public void Update()
    {
    }

    protected override void CloseState(string state)
    {
        NJ_EContentState eState = (NJ_EContentState)Enum.Parse(typeof(NJ_EContentState), state);
        switch (eState)
        {
            case NJ_EContentState.Content1:
                break;
            case NJ_EContentState.Content2:
                break;
            case NJ_EContentState.Content3:
                break;
            default:
                break;
        }
    }

    protected override void OpenState(string state)
    {
        NJ_EContentState eState = (NJ_EContentState)Enum.Parse(typeof(NJ_EContentState), state);
        switch (eState)
        {
            case NJ_EContentState.Content1:
                // 카운팅 관련 함수 호출
                RunCountdown();
                break;
            case NJ_EContentState.Content2:
                DisplayTutorialImage();
                break;
            case NJ_EContentState.Content3:

                break;
            default:
                break;
        }
    }

    private void RunCountdown()
    {
        // 실행중인 코루틴이 있다면 멈춘다.
        if(null != countdownCoroutine)
        {
            StopCoroutine(countdownCoroutine);
        }
        // 새 코루틴 시작
        countdownCoroutine = StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        int count = countdownImages.Count;

        while (count > 0)
        {
            // 이미지 출력 로직
            countdownDisplay.sprite = countdownImages[count-1];
            // 카운트 다운 로직
            count--;
            yield return new WaitForSeconds(1f);
        }

        if(0 == count)
        {
            // Explain 화면으로 이동
            UIManager.Instance.MoveNextPage();  // Page : Counting -> Explain
        }
    }

    private void DisplayTutorialImage()
    {
        //
    }

    private void PlayJumpGame()
    {

    }
}
