using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Yans.UI.UIScreens;

namespace Yans.UI
{
    public abstract class ScreenInstantiator : MonoBehaviour, IScreenInstantiator
    {
        #region public methods
        public abstract UniTask<T> InstantiateScreen<T>(Transform parent, ScreenOrientation screenOrientation) where T : UIScreen;

        public abstract UniTask<UIScreen> InstantiateScreen(Type type, Transform parent, ScreenOrientation screenOrientation);

        public abstract void CleanUpScreen(UIScreen screen);
        #endregion

        #region protected methods

        protected ScreenOrientation GetFallbackOrientation(ScreenOrientation orientation)
        {
            return orientation switch
            {
                ScreenOrientation.Portrait => ScreenOrientation.AutoRotation,
                ScreenOrientation.PortraitUpsideDown => ScreenOrientation.Portrait,
                ScreenOrientation.LandscapeLeft => ScreenOrientation.AutoRotation,
                ScreenOrientation.LandscapeRight => ScreenOrientation.LandscapeLeft,
                ScreenOrientation.AutoRotation => ScreenOrientation.AutoRotation,
                _ => ScreenOrientation.AutoRotation,
            };
        }

        #endregion
    }
}