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
    [Tooltip("ī��Ʈ �ٿ��� ��µ� �̹��� ��ü")]
    [SerializeField] Image countdownDisplay = null;
    [Tooltip("ī��Ʈ �ٿ ����� �ؽ�ó ���ҽ���")]
    [SerializeField] List<Sprite> countdownImages = new List<Sprite>();
    [NonSerialized] private Coroutine countdownCoroutine;   // �������� �ڷ�ƾ ����

    [Header("Content 3")]
    [Tooltip("���� Ŭ���� ����")]
    [SerializeField][ReadOnly] bool isClear = true;

    public void Start()
    {
        if (0 == countdownImages.Count || null == countdownDisplay)
        {
            Debug.LogError("ī��Ʈ �ٿ ����� ��ü �Ǵ� �̹����� �������� �ʾҽ��ϴ�.");
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
                // ī���� ���� �Լ� ȣ��
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
        // �������� �ڷ�ƾ�� �ִٸ� �����.
        if(null != countdownCoroutine)
        {
            StopCoroutine(countdownCoroutine);
        }
        // �� �ڷ�ƾ ����
        countdownCoroutine = StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        int count = countdownImages.Count;

        while (count > 0)
        {
            // �̹��� ��� ����
            countdownDisplay.sprite = countdownImages[count-1];
            // ī��Ʈ �ٿ� ����
            count--;
            yield return new WaitForSeconds(1f);
        }

        if(0 == count)
        {
            // Explain ȭ������ �̵�
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
