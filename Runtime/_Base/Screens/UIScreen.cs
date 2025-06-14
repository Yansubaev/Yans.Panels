using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yans.ViewModels;

namespace Yans.UI.UIScreens
{
    public abstract class UIScreen : UIBehaviour, IViewModelOwner
    {
        #region public properties
        public Canvas Canvas => _canvas;
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster;
        public RectTransform TransitionRoot => _transitionRoot;
        public CanvasGroup FadeRoot => _fadeRoot;
        public bool IsLifecycleStarted { get; private set; }
        public bool IsLifecycleResumed { get; private set; }
        #endregion
        internal List<IScreenResultListener> ResultListeners => _resultListeners;
        protected IViewModelProvider ViewModelProvider => _viewModelProvider;

        protected List<IScreenResultListener> _resultListeners;

        #region private fields

        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private GraphicRaycaster _graphicRaycaster;

        [SerializeField]
        private RectTransform _transitionRoot;

        [SerializeField]
        private CanvasGroup _fadeRoot;

        private IViewModelProvider _viewModelProvider;
        private string _instanceId;
        #endregion

        public string GetInstanceId()
        {
            return _instanceId;
        }

        #region internal methods

        internal void Create(IViewModelProvider viewModelProvider, string instanceId = null, List<IScreenResultListener> resultListeners = null)
        {
#if LOG_SCREEN_LIFECYCLE
            Debug.Log($"<color=green>[SCREEN] {name}.Create</color>", gameObject);
#endif
            _viewModelProvider = viewModelProvider;
            _instanceId = instanceId ?? $"{GetType().Name}-{System.Guid.NewGuid()}";
            _resultListeners = resultListeners ?? new List<IScreenResultListener>();

            CreateLifecycle();
        }

        internal void CreateLifecycle()
        {
            OnCreated();
        }

        internal void StartLifecycle()
        {
            if (IsLifecycleStarted) return;

#if LOG_SCREEN_LIFECYCLE
            Debug.Log($"<color=blue>[SCREEN] {name}.StartScreen</color>", gameObject);
#endif
            Canvas.enabled = true;

            OnStarted();
            IsLifecycleStarted = true;
        }

        internal void ResumeLifecycle()
        {
            if (IsLifecycleResumed) return;

#if LOG_SCREEN_LIFECYCLE
            Debug.Log($"<color=cyan>[SCREEN] {name}.ResumeScreen</color>", gameObject);
#endif

            GraphicRaycaster.enabled = true;

            OnResumed();
            IsLifecycleResumed = true;
        }

        internal void PauseLifecycle()
        {
            if (!IsLifecycleResumed) return;

#if LOG_SCREEN_LIFECYCLE
            Debug.Log($"<color=orange>[SCREEN] {name}.PauseScreen</color>", gameObject);
#endif

            GraphicRaycaster.enabled = false;

            OnPaused();
            IsLifecycleResumed = false;
        }

        internal void StopLifecycle()
        {
            if (!IsLifecycleStarted) return;

#if LOG_SCREEN_LIFECYCLE
            Debug.Log($"<color=yellow>[SCREEN] {name}.StopScreen</color>", gameObject);
#endif
            Canvas.enabled = false;

            OnStopped();
            IsLifecycleStarted = false;
        }

        internal void Close()
        {
#if LOG_SCREEN_LIFECYCLE
            Debug.Log($"<color=magenta>[SCREEN] {name}.Close</color>", gameObject);
#endif

            OnClosed();
        }

        #endregion

        public void AddScreenResultListener(IScreenResultListener listener)
        {
            if (listener == null || _resultListeners.Contains(listener)) return;
            _resultListeners.Add(listener);
        }

        #region protected methods
        protected virtual void OnCreated() { }

        protected virtual void OnStarted() { }

        protected virtual void OnResumed() { }

        protected virtual void OnPaused() { }

        protected virtual void OnStopped() { }

        protected virtual void OnClosed() { }

        protected void SendResultToListeners(ScreenResult result)
        {
            foreach (var listener in _resultListeners)
            {
                listener.OnScreenResult(this, result);
            }
        }

        #endregion
    }
}