using Cysharp.Threading.Tasks;
using Yans.UI.UIScreens;

namespace Yans.UI.Transitions
{
    public class EmptyTransition : ITransition
    {
        public UniTask Play(UIPanel from, UIPanel to)
        {
            // No transition logic, just return a completed task
            return UniTask.CompletedTask;
        }
    }
}