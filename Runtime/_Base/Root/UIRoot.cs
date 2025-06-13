using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yans.UI
{
    /// <summary>
    /// Root object for UI screens such as panels and popups. Should be placed somewhere on scene.
    /// Use it in screen service to attach screens to specified containers
    /// </summary>
    [DisallowMultipleComponent]
    public class UIRoot : MonoBehaviour
    {
        #region public properties
        public Canvas Canvas => _canvas;
        public CanvasScaler CanvasScaler => _canvasScaler;
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster;
        public RectTransform ScreenRoot => _screenRoot;
        public ScreenOrientation CurrentOrientation => _currentOrientation;
        #endregion

        #region private fields

        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private CanvasScaler _canvasScaler;

        [SerializeField]
        private GraphicRaycaster _graphicRaycaster;

        [SerializeField]
        private RectTransform _screenRoot;

        private ScreenOrientation _currentOrientation;
        private readonly List<IOrientationChangeListener> _orientationListeners = new();
        #endregion

        #region public methods

        public void AddOrientationListener(IOrientationChangeListener listener)
        {
            if (!_orientationListeners.Contains(listener))
            {
                _orientationListeners.Add(listener);
            }
        }

        public void RemoveOrientationListener(IOrientationChangeListener listener)
        {
            _orientationListeners.Remove(listener);
        }

        #endregion

        #region private methods

        private void Awake()
        {
            _currentOrientation = Screen.orientation;
        }

        private void Update()
        {
            CheckOrientation();
        }

        private void CheckOrientation()
        {
            if (_currentOrientation != Screen.orientation)
            {
                _currentOrientation = Screen.orientation;
                NotifyOrientationListeners();
            }
        }

        private void NotifyOrientationListeners()
        {
            foreach (var listener in _orientationListeners)
            {
                listener.OnOrientationChanged(_currentOrientation);
            }
        }

        #endregion
    }
}