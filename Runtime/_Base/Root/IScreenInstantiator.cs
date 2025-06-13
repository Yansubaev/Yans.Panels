using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Yans.UI.UIScreens;

namespace Yans.UI
{
    public interface IScreenInstantiator
    {
        UniTask<T> InstantiateScreen<T>(Transform parent, ScreenOrientation screenOrientation) where T : UIScreen;
        UniTask<UIScreen> InstantiateScreen(Type type, Transform parent, ScreenOrientation screenOrientation);
        void CleanUpScreen(UIScreen screen);
    }
}
