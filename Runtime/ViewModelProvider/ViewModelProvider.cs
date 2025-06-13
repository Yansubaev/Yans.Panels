using System.Collections.Generic;
using Yans.UI;

namespace Yans.ViewModels
{
    public class ViewModelProvider : IViewModelProvider
    {
        private Dictionary<string, List<ViewModel>> _viewModels = new();

        public T Get<T>(IViewModelOwner lifecycleOwner) where T : ViewModel
        {
            string instanceId = lifecycleOwner.GetInstanceId();
            if (!_viewModels.TryGetValue(instanceId, out var viewModels))
            {
                viewModels = new List<ViewModel>();
                _viewModels[instanceId] = viewModels;
            }

            foreach (var vm in viewModels)
            {
                if (vm is T typedVm)
                {
                    return typedVm;
                }
            }

            var newViewModel = CreateViewModelInstance<T>();
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

        protected virtual V CreateViewModelInstance<V>() where V : ViewModel
        {
            return System.Activator.CreateInstance<V>();;
        }
    }
}