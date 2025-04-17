using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInOut : MonoBehaviour
{
    [Tooltip("효과를 적용할 타겟")]
    [ReadOnly][SerializeField] GameObject target;
    [Tooltip("Zoom 효과로 인해 변경될 타겟의 스케일")]
    [SerializeField] Vector3 zoomScale  = Vector3.one;
    //[Tooltip("Zoom 효과의 수행 시간")]
    //[SerializeField] float effectDuration;
    [Tooltip("실행중인 코루틴 참조")] 
    [ReadOnly][SerializeField] Coroutine effectCoroutine;

    protected void Awake()
    {
        target = this.gameObject;
    }

    protected void Start()
    {
        if (null == target) Debug.LogError("Target not set");
    }

    public void RunEffect(float duration)
    {
        effectCoroutine = StartCoroutine(OnEffect(duration));
    }

    private IEnumerator OnEffect(float duration)
    {
        // 스케일 변경
        Vector3 originalScale = target.GetComponent<RectTransform>().localScale;
        float elapsedTime = 0f;
        while (true)
        {
            // 스케일 보간
            Vector3 newScale = GetNewLerpScale(originalScale, zoomScale, elapsedTime, duration / 2f);
            // 스케일 적용
            target.GetComponent<RectTransform>().localScale = newScale;
            // 목표치에 도달했다면 반복문 탈출
            if (zoomScale == newScale)
            {
                break;
            }
            // 시간 누적
            elapsedTime += Time.deltaTime;
            yield return null;  // 프레임 끝날때까지 대기
        }
        elapsedTime = 0f;

        // 스케일 복귀
        while (true)
        {
            // 스케일 보간
            Vector3 newScale = GetNewLerpScale(zoomScale, originalScale, elapsedTime, duration / 2f);
            // 스케일 적용
            target.GetComponent<RectTransform>().localScale = newScale;
            // 목표치에 도달했다면 반복문 탈출
            if (originalScale == newScale) break;
            // 시간 누적
            elapsedTime += Time.deltaTime;
            yield return null;  // 프레임 끝날때까지 대기
        }
        elapsedTime = 0f;

        yield return null;  // 프레임 끝날때까지 대기
    }

    private Vector3 GetNewLerpScale(Vector3 originalScale, Vector3 zoomScale, float elapsedTime, float duration)
    {
        Vector3 newScale = Vector3.Lerp(originalScale, zoomScale, elapsedTime / duration);

        float newScaleX = originalScale.x < zoomScale.x ? Mathf.Clamp(newScale.x, originalScale.x, zoomScale.x) : Mathf.Clamp(newScale.x, zoomScale.x, originalScale.x);
        float newScaleY = originalScale.y < zoomScale.y ? Mathf.Clamp(newScale.y, originalScale.y, zoomScale.y) : Mathf.Clamp(newScale.y, zoomScale.y, originalScale.y);
        float newScaleZ = originalScale.z < zoomScale.z ? Mathf.Clamp(newScale.z, originalScale.z, zoomScale.z) : Mathf.Clamp(newScale.z, zoomScale.z, originalScale.z);

        return new Vector3(newScaleX, newScaleY, newScaleZ);
    }


}
