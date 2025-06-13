using Yans.UI;

namespace Yans.ViewModels
{
    public interface IViewModelProvider
    {
        VM Get<VM>(IViewModelOwner lifecycleOwner) where VM : ViewModel;
        void Clear(IViewModelOwner lifecycleOwner);
    }
}
