using System;
using UnityEngine;

namespace Yans.UI.UIScreens
{
    public abstract class UIPanel : UIScreen
    {
        #region public fields
        public RectTransform Wrapper;
        #endregion

        #region private fields

        [SerializeField]
        private ScreenSide _useSafeAreaOn = ScreenSide.Left | ScreenSide.Right | ScreenSide.Top | ScreenSide.Bottom;

        [SerializeField]
        private RectTransform _wrapper;

        #endregion

        public ScreenSide UseSafeAreaOn
        {
            get => _useSafeAreaOn;
            set
            {
                _useSafeAreaOn = value;
                ResizePanelToSafeArea(Screen.safeArea, new Vector2(Screen.width, Screen.height));
            }
        }

        #region protected methods

        protected override void OnStarted()
        {
            base.OnStarted();
            ResizePanelToSafeArea(Screen.safeArea, new Vector2(Screen.width, Screen.height));
        }

        protected virtual void ResizePanelToSafeArea(Rect safeArea, Vector2 screenSize)
        {
            if (_wrapper == null) return;
            var scale = 1 / Canvas.scaleFactor;

            Vector2 offsetMin = _wrapper.offsetMin;
            Vector2 offsetMax = _wrapper.offsetMax;

            if (_useSafeAreaOn.HasFlag(ScreenSide.Left))
                offsetMin.x = safeArea.x * scale;

            if (_useSafeAreaOn.HasFlag(ScreenSide.Bottom))
                offsetMin.y = safeArea.y * scale;

            if (_useSafeAreaOn.HasFlag(ScreenSide.Right))
                offsetMax.x = -((screenSize.x - safeArea.width - safeArea.x) * scale);

            if (_useSafeAreaOn.HasFlag(ScreenSide.Top))
                offsetMax.y = -((screenSize.y - safeArea.height - safeArea.y) * scale);

            _wrapper.offsetMin = offsetMin;
            _wrapper.offsetMax = offsetMax;
        }

        #endregion

        [Flags]
        public enum ScreenSide
        {
            None = 0,
            Left = 1 << 0,
            Right = 1 << 1,
            Top = 1 << 2,
            Bottom = 1 << 3
        }
    }
}