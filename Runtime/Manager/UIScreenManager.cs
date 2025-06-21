using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yans.UI.Transitions;
using Yans.UI.UIScreens;
using Yans.ViewModels;

namespace Yans.UI
{
    public class UIScreenManager : IScreenManager, IOrientationChangeListener
    {
        #region private fields
        private ScreenOrientation _currentScreenOrientation;
        private readonly List<UIScreen> _stack = new();
        private readonly ITransitionResolver _transitionResolver;
        private readonly UIRoot _uiRoot;
        private readonly IScreenInstantiator _screenInstantiator;
        private readonly IViewModelProvider _viewModelProvider;
        #endregion

        public UIScreenManager(
            UIRoot uIRoot,
            IScreenInstantiator screenInstantiator,
            ITransitionResolver transitionResolver,
            IViewModelProvider viewModelProvider = null)
        {
            _uiRoot = uIRoot;
            _screenInstantiator = screenInstantiator;
            _transitionResolver = transitionResolver;
            _viewModelProvider = viewModelProvider ?? new ViewModelProvider();

            _currentScreenOrientation = uIRoot.CurrentOrientation;

            _uiRoot.AddOrientationListener(this);
        }

        #region public methods

        public async UniTask<T> OpenScreen<T>() where T : UIScreen
        {
            var newScreen = await _screenInstantiator.InstantiateScreen<T>(_uiRoot.ScreenRoot, _currentScreenOrientation);
            var prevScreen = _stack.LastOrDefault();
            _stack.Add(newScreen);
            newScreen.Create(_viewModelProvider);

            ContinueOpenScreen(newScreen, prevScreen).Forget();
            
            return newScreen;
        }

        public void CloseScreen(UIScreen screen)
        {
            if (!_stack.Contains(screen))
            {
                Debug.LogWarning($"Screen {screen.GetType().Name} is not open.");
                return;
            }

            var isTopScreen = _stack.Last() == screen;
            _stack.Remove(screen);
            ContinueCloseScreen(screen, isTopScreen).Forget();
        }

        public void CloseTop()
        {
            if (_stack.Count == 0)
            {
                Debug.LogWarning("No screens to close.");
                return;
            } else {
                var topScreen = _stack.Last();
                CloseScreen(topScreen);
            }
        }

        public void OnOrientationChanged(ScreenOrientation newOrientation)
        {
            _currentScreenOrientation = newOrientation;
            RebuildScreens();
        }

        public T GetScreen<T>() where T : UIScreen
        {
            var screen = _stack.OfType<T>().LastOrDefault();
            if (screen == null)
            {
                throw new System.InvalidOperationException($"Screen of type {typeof(T).Name} is not found in the stack.");
            }
            return screen;
        }

        public bool TryGetScreen<T>(out T screen) where T : UIScreen
        {
            screen = _stack.OfType<T>().LastOrDefault();
            return screen != null;
        }

        public void CloseAll()
        {
            if (_stack.Count == 0)
            {
                return;
            }

            foreach (var screen in _stack)
            {
                screen.PauseLifecycle();
                screen.StopLifecycle();
                screen.Close();
            }
            _stack.Clear();
        }

        #endregion

        #region private methods
        private async UniTask<UIScreen> ContinueOpenScreen(UIScreen newScreen, UIScreen prevScreen)
        {
            SafePauseLifecycle(prevScreen);
            SafeStartLifecycle(newScreen);

            await HandlePanelsTransition(prevScreen, newScreen);

            if (newScreen != null && !newScreen.GetType().IsSubclassOf(typeof(UIPopup)))
                SafeStopLifecycle(prevScreen);

            SafeResumeLifecycle(newScreen);

            return newScreen;
        }

        private async UniTask ContinueCloseScreen(UIScreen screen, bool isTopScreen)
        {
            SafePauseLifecycle(screen);

            if (isTopScreen)
            {
                var newTopScreen = _stack.LastOrDefault();
                SafeStartLifecycle(newTopScreen);

                await HandlePanelsTransition(screen, newTopScreen);

                SafeResumeLifecycle(newTopScreen);
            }

            SafeStopLifecycle(screen);
            CleanupScreen(screen);
        }

        private void CleanupScreen(UIScreen screen)
        {
            screen.Close();
            _screenInstantiator.CleanUpScreen(screen);
        }

        private async UniTask HandlePanelsTransition(UIScreen fromScreen, UIScreen toScreen)
        {
            if (fromScreen is not UIPanel fomPanel || toScreen is not UIPanel toPanel) return;

            var transition = _transitionResolver.Resolve(fomPanel.GetType(), toPanel.GetType());
            await transition.Play(fomPanel, toPanel);
        }

        private async void RebuildScreens()
        {
            var screensToRebuild = _stack.ToList();
            _stack.Clear();

            foreach (var screen in screensToRebuild)
            {
                try
                {
                    await RebuildScreen(screen);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to rebuild screen {screen.GetType().Name}: {e}");
                }
            }
        }

        private async UniTask RebuildScreen(UIScreen oldScreen)
        {
            var newScreen = await _screenInstantiator.InstantiateScreen(oldScreen.GetType(), _uiRoot.ScreenRoot, _currentScreenOrientation);

            if (oldScreen == newScreen)
            {
                _stack.Add(oldScreen);
                return;
            }

            TransferConfiguration(oldScreen, newScreen);
            _stack.Add(newScreen);
            CleanupScreen(oldScreen);
        }

        private void TransferConfiguration(UIScreen oldScreen, UIScreen newScreen)
        {
            var isLifecycleStarted = oldScreen.IsLifecycleStarted;
            var isLifecycleResumed = oldScreen.IsLifecycleResumed;

            oldScreen.PauseLifecycle();
            oldScreen.StopLifecycle();

            newScreen.Create(_viewModelProvider, oldScreen.GetInstanceId(), oldScreen.ResultListeners);

            if (isLifecycleStarted)
            {
                newScreen.StartLifecycle();
                if (isLifecycleResumed)
                {
                    newScreen.ResumeLifecycle();
                }
            }
        }

        private void SafePauseLifecycle(UIScreen screen)
        {
            if (screen != null)
                screen.PauseLifecycle();
        }

        private void SafeResumeLifecycle(UIScreen screen)
        {
            if (screen != null)
                screen.ResumeLifecycle();
        }

        private void SafeStartLifecycle(UIScreen screen)
        {
            if (screen != null)
                screen.StartLifecycle();
        }

        private void SafeStopLifecycle(UIScreen screen)
        {
            if (screen != null)
                screen.StopLifecycle();
        }

        #endregion
    }
}