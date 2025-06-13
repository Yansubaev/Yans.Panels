using System.Collections.Generic;
using Yans.UI;

namespace Yans.ViewModels
{
    public class ViewModelProvider : IViewModelProvider
    {
        private Dictionary<string, List<ViewModel>> _viewModels = new();

        public VM Get<VM>(IViewModelOwner lifecycleOwner) where VM : ViewModel
        {
            string instanceId = lifecycleOwner.GetInstanceId();
            if (!_viewModels.TryGetValue(instanceId, out var viewModels))
            {
                viewModels = new List<ViewModel>();
                _viewModels[instanceId] = viewModels;
            }

            foreach (var vm in viewModels)
            {
                if (vm is VM typedVm)
                {
                    return typedVm;
                }
            }

            var newViewModel = System.Activator.CreateInstance<VM>();
            newViewModel.Create();
            viewModels.Add(newViewModel);
            return newViewModel;
        }

        public void Clear(IViewModelOwner lifecycleOwner)
        {
            string instanceId = lifecycleOwner.GetInstanceId();
            if (_viewModels.TryGetValue(instanceId, out var viewModels))
            {
                foreach (var vm in viewModels)
                {
                    vm.Abort();
                }
                viewModels.Clear();
            }
        }
    }
}