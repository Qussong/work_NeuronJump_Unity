using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GH
{
    public interface IScreenInfo
    {
        // ȭ�� ����
        Vector2 GetScreenPixel();   // ȭ���� �ȼ� ������ ��ȯ�Ѵ�. ���� ȭ���� ũ�⸦ �ȼ� ������ ����
        Vector2 GetScreenResolution();  // ȭ���� �ػ󵵸� ��ȯ�Ѵ�. ȭ���� �������� ũ�⸦ �ǹ��Ѵ�.
        float GetScreenAspectRatio();   // ȭ���� ��Ⱦ�� ��ȯ�Ѵ�. ����/����
    }
}

