using Cysharp.Threading.Tasks;
using Yans.UI.UIScreens;

namespace Yans.UI.Transitions
{
    public interface ITransition
    {
        public UniTask Play(UIPanel from, UIPanel to);
    }
}
