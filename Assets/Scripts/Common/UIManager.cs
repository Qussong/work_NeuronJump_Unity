using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GH
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class UIManager : SingletonTemplate<UIManager>, IUIManager, IScreenInfo//, IScenePersistent
    {
        // UI Manager Interface
        #region IUIManager

        public event EventHandler MoveHomeHandler;
        public event EventHandler MovePrevPageHandler;
        public event EventHandler MoveNextPageHandler;
        public event EventHandler<PageChangedEventArgs> PageChangedHandler;

        [Tooltip("Page ������ ����ϴ� Panel ��ü���� �����ϴ� �����̳�")]
        [SerializeField] private List<GameObject> pageList = new List<GameObject>();
        public List<GameObject> PageList
        {
            get { return pageList; }
        }

        private static readonly object pageKeeper = new object();
        [Tooltip("���� ������ �ε��� ��")]
        [ReadOnly] private int curPageIdx = 0;
        public int CurPageIdx
        {
            get
            {
                lock (pageKeeper) return curPageIdx;
            }
            set
            {
                lock (pageKeeper) curPageIdx = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            MoveHomeHandler += OnMoveHome;
            MovePrevPageHandler += OnMovePrevPage;
            MoveNextPageHandler += OnMoveNextPage;
            PageChangedHandler += OnPageChanged;
        }

        public virtual void InitUI()
        {
            for (int i = 0; i < pageList.Count; ++i)
            {
                DeactivateUI(i);
            }
            ActivateUI(0); // ù��° ������ Ȱ��ȭ
        }

        #region Page Change Method

        public virtual void MoveHome()
        {
            //Debug.Log($"Call {this.GetType().Name}.MoveHome()");
            if (null == MoveHomeHandler) return;
            MoveHomeHandler.Invoke(this, EventArgs.Empty);
        }

        public virtual void MovePrevPage()
        {
            //Debug.Log($"Call {this.GetType().Name}.MovePrePage()");
            if (null == MovePrevPageHandler) return;
            MovePrevPageHandler.Invoke(this, EventArgs.Empty);
        }

        public virtual void MoveNextPage()
        {
            //Debug.Log($"Call {this.GetType().Name}.MoveNextPage()");
            if (null == MoveNextPageHandler) return;
            MoveNextPageHandler.Invoke(this, EventArgs.Empty);
        }

        public void PageChanged(PageChangedEventArgs e)
        {
            //Debug.Log($"Call {this.GetType().Name}.PageChanged()");
            if (null == PageChangedHandler) return;
            PageChangedHandler?.Invoke(this, e);
        }
        #endregion

        #region Page Change Handler Method

        public virtual void OnMoveHome(object sender, EventArgs e)
        {
            PageChanged(new PageChangedEventArgs(CurPageIdx, 0));
        }

        public virtual void OnMovePrevPage(object sender, EventArgs e)
        {
            int maxPageCnt = PageList.Count;
            int newPageIdx = CurPageIdx - 1;
            // ù��° �������� ���
            if (newPageIdx < 0) return;
            // ������ �̵�
            PageChanged(new PageChangedEventArgs(CurPageIdx, newPageIdx));
        }

        public virtual void OnMoveNextPage(object sender, EventArgs e)
        {
            int maxPageCnt = PageList.Count;
            int newPageIdx = CurPageIdx + 1;
            // ������ �������� ���
            if (newPageIdx >= maxPageCnt) return;
            // ������ �̵�
            PageChanged(new PageChangedEventArgs(CurPageIdx, newPageIdx));
        }

        public virtual void OnPageChanged(object sender, PageChangedEventArgs e)
        {
            DeactivateUI(e.PrevPageIdx);
            ActivateUI(e.NewPageIdx);
            CurPageIdx = e.NewPageIdx;

            // ������ �̵��� ���� �߰� ������ �ڽ� Ŭ�������� ����

        }
        #endregion

        #region UI Status Method

        public virtual bool HasUIElement<T>(T enumId) where T : Enum
        {
            if (enumId is T page)
            {
                return HasUIElement(Convert.ToInt32(page));
            }
            Debug.LogError("An invalid type has been provided.");
            return false;
        }

        protected virtual bool HasUIElement(int enumId)
        {
            if (null != PageList[enumId]) return true;

            Debug.LogError("An invalid parameter value has been provided.");
            return false;
        }
        #endregion 

        #region UI Active Method

        public virtual void ActivateUI<T>(T enumId) where T : Enum
        {
            if (enumId is T page)
            {
                ActivateUI(Convert.ToInt32(page));
            }
        }

        public virtual void ActivateUI(int enumId)
        {
            GameObject panel = PageList[enumId];
            if (null == panel) return;

            panel.SetActive(true);
        }

        public virtual void DeactivateUI<T>(T enumId) where T : Enum
        {
            if (enumId is T page)
            {
                DeactivateUI(Convert.ToInt32(page));
            }
        }

        public virtual void DeactivateUI(int enumId)
        {
            GameObject panel = PageList[enumId];
            if (null == panel) return;

            panel.SetActive(false);
        }
        #endregion

        #endregion

        // Screen Info Interface
        #region ScreenInfo

        public Vector2 GetScreenPixel()
        {
            return new Vector2(Screen.width, Screen.height);
        }

        public Vector2 GetScreenResolution()
        {
            return new Vector2(Display.main.systemWidth, Display.main.systemHeight);
        }

        public float GetScreenAspectRatio()
        {
            return (float)Screen.width / (float)Screen.height;
        }

        #endregion
    }
}

