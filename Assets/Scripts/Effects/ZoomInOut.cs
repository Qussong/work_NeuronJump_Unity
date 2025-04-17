using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInOut : MonoBehaviour
{
    [Tooltip("ȿ���� ������ Ÿ��")]
    [ReadOnly][SerializeField] GameObject target;
    [Tooltip("Zoom ȿ���� ���� ����� Ÿ���� ������")]
    [SerializeField] Vector3 zoomScale  = Vector3.one;
    //[Tooltip("Zoom ȿ���� ���� �ð�")]
    //[SerializeField] float effectDuration;
    [Tooltip("�������� �ڷ�ƾ ����")] 
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
        // ������ ����
        Vector3 originalScale = target.GetComponent<RectTransform>().localScale;
        float elapsedTime = 0f;
        while (true)
        {
            // ������ ����
            Vector3 newScale = GetNewLerpScale(originalScale, zoomScale, elapsedTime, duration / 2f);
            // ������ ����
            target.GetComponent<RectTransform>().localScale = newScale;
            // ��ǥġ�� �����ߴٸ� �ݺ��� Ż��
            if (zoomScale == newScale)
            {
                break;
            }
            // �ð� ����
            elapsedTime += Time.deltaTime;
            yield return null;  // ������ ���������� ���
        }
        elapsedTime = 0f;

        // ������ ����
        while (true)
        {
            // ������ ����
            Vector3 newScale = GetNewLerpScale(zoomScale, originalScale, elapsedTime, duration / 2f);
            // ������ ����
            target.GetComponent<RectTransform>().localScale = newScale;
            // ��ǥġ�� �����ߴٸ� �ݺ��� Ż��
            if (originalScale == newScale) break;
            // �ð� ����
            elapsedTime += Time.deltaTime;
            yield return null;  // ������ ���������� ���
        }
        elapsedTime = 0f;

        yield return null;  // ������ ���������� ���
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
