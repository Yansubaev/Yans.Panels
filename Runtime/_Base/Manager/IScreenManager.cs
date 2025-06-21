using Cysharp.Threading.Tasks;
using Yans.UI.UIScreens;

namespace Yans.UI
{
    public interface IScreenManager
    {
        UniTask<T> OpenScreen<T>() where T : UIScreen;
        void CloseScreen(UIScreen screen);
        void CloseTop();
        void CloseAll();
        T GetScreen<T>() where T : UIScreen;
        bool TryGetScreen<T>(out T screen) where T : UIScreen;
    }
}