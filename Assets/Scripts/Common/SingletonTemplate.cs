using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GH
{
    // Marker Interface
    public interface IScenePersistent { }

    // Singleton Template
    public class SingletonTemplate<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;
        private static readonly object locker = new object();

        public static T Instance
        {
            get
            {
                if (null == instance)
                {
                    lock (locker)
                    {
                        instance = FindByType();
                        if (null == instance)
                        {
                            GameObject obj = new GameObject(typeof(T).Name);
                            instance = obj.AddComponent<T>();

                            if (instance is IScenePersistent)
                            {
                                DontDestroyOnLoad(obj);
                            }
                        }
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            // �ߺ��� ��ü ���� ����
            if (instance != null && instance != this)
            {
                Debug.LogWarning("Duplicate " + typeof(T).Name + " instance found, destroying this one.");
                Destroy(gameObject);
                return;
            }

            instance = this as T;   // this�� T�� ����ȯ�� �Ұ����ϸ� null ��ȯ
            if (instance is IScenePersistent)
            {
                DontDestroyOnLoad(gameObject);
            }

            Debug.Log("Create \"" + typeof(T).Name + "\" Singleton Class");
        }

        // �̱��� ��Ȱ��ȭ
        public static void DisableSingleton()
        {
            Debug.Log("Disable " + typeof(T).Name);
            if (null != instance)
            {
                instance.gameObject.SetActive(false);
            }
        }

        // �̱��� Ȱ��ȭ
        public static void EnableSingleton()
        {
            Debug.Log("Enable " + typeof(T).Name);
            if (null != instance)
            {
                instance.gameObject.SetActive(true);
            }
        }

        // �̱��� ����
        public static void DestroySingleton()
        {
            if (instance != null)
            {
                Debug.Log("Destroy " + typeof(T).Name);
                Destroy(instance.gameObject);
                instance = null;
            }
        }

        // T Ÿ�� ������Ʈ Ž��
        private static T FindByType()
        {
            List<T> allObjects = new List<T>();
            Scene curScene = SceneManager.GetActiveScene();
            GameObject[] rootObjects = curScene.GetRootGameObjects();

            foreach (var rootObject in rootObjects)
            {
                allObjects.AddRange(rootObject.GetComponentsInChildren<T>(true));
            }

            if (1 == allObjects.Count)
            {
                return allObjects[0];
            }
            else if (1 < allObjects.Count)
            {
                Debug.LogError("Multiple instance of " + typeof(T).Name + " exist.");
            }

            return null;
        }

    }
}
