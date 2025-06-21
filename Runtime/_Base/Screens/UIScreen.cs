using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yans.ViewModels;
using Cysharp.Threading.Tasks;

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

        private UniTaskCompletionSource<bool> _startCompletionSource;
        private UniTaskCompletionSource<bool> _resumeCompletionSource;
        private UniTaskCompletionSource<bool> _pauseCompletionSource;
        private UniTaskCompletionSource<bool> _stopCompletionSource;
        private UniTaskCompletionSource<bool> _closeCompletionSource;
        #endregion

        #region public methods

        public string GetInstanceId()
        {
            return _instanceId;
        }

        public void AddScreenResultListener(IScreenResultListener listener)
        {
            if (listener == null || _resultListeners.Contains(listener)) return;
            _resultListeners.Add(listener);
        }

        public void RemoveScreenResultListener(IScreenResultListener listener)
        {
            if (listener == null || !_resultListeners.Contains(listener)) return;
            _resultListeners.Remove(listener);
        }

        #endregion

        #region internal methods

        internal void Create(IViewModelProvider viewModelProvider, string instanceId = null, List<IScreenResultListener> resultListeners = null)
        {
#if LOG_SCREEN_LIFECYCLE
            Debug.Log($"<color=green>[SCREEN] {name}.Create</color>", gameObject);
#endif
            _viewModelProvider = viewModelProvider;
            _instanceId = instanceId ?? $"{GetType().Name}-{System.Guid.NewGuid()}";
            _resultListeners = resultListeners ?? new List<IScreenResultListener>();

            // Initialize completion sources for awaiters
            _startCompletionSource = new UniTaskCompletionSource<bool>();
            _resumeCompletionSource = new UniTaskCompletionSource<bool>();
            _pauseCompletionSource = new UniTaskCompletionSource<bool>();
            _stopCompletionSource = new UniTaskCompletionSource<bool>();
            _closeCompletionSource = new UniTaskCompletionSource<bool>();

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
            _startCompletionSource.TrySetResult(true);
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
            _resumeCompletionSource.TrySetResult(true);
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
            _pauseCompletionSource.TrySetResult(true);
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
            _stopCompletionSource.TrySetResult(true);
        }

        internal void Close()
        {
#if LOG_SCREEN_LIFECYCLE
            Debug.Log($"<color=magenta>[SCREEN] {name}.Close</color>", gameObject);
#endif

            OnClosed();
            _closeCompletionSource.TrySetResult(true);
        }

        #endregion

        #region protected methods

        protected void ClearScreenResultListeners()
        {
            _resultListeners.Clear();
        }

        protected virtual void OnCreated() { }

        protected virtual void OnStarted() { }

        protected virtual void OnResumed() { }

        protected virtual void OnPaused() { }

        protected virtual void OnStopped() { }

        protected virtual void OnClosed() { }

        protected void SendResultToListeners(ScreenResult result)
        {
            foreach (var listener in new List<IScreenResultListener>(_resultListeners))
            {
                listener.OnScreenResult(this, result);
            }
        }

        #endregion

        #region awaitable methods

        public UniTask<bool> WaitForStartAsync()
        {
            if (IsLifecycleStarted) return UniTask.FromResult(true);
            return _startCompletionSource.Task;
        }

        public UniTask<bool> WaitForResumeAsync()
        {
            if (IsLifecycleResumed) return UniTask.FromResult(true);
            return _resumeCompletionSource.Task;
        }

        public UniTask<bool> WaitForPauseAsync()
        {
            if (!IsLifecycleResumed) return UniTask.FromResult(true);
            return _pauseCompletionSource.Task;
        }

        public UniTask<bool> WaitForStopAsync()
        {
            if (!IsLifecycleStarted) return UniTask.FromResult(true);
            return _stopCompletionSource.Task;
        }

        public UniTask<bool> WaitForCloseAsync()
        {
            // Await close completion
            return _closeCompletionSource.Task;
        }

        #endregion
    }
}