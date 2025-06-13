using UnityEngine;

namespace Yans.UI
{
    public interface IOrientationChangeListener
    {
        void OnOrientationChanged(ScreenOrientation newOrientation);
    }
}