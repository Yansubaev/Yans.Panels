using System;
using System.Collections.Generic;

namespace Yans.UI.Transitions
{
    public class TransitionResolver : ITransitionResolver
    {
        private readonly Dictionary<(Type from, Type to), ITransition> _transitions;
        private readonly ITransition _defaultTransition;

        public TransitionResolver(ITransition defaultTransition = null)
        {
            _transitions = new Dictionary<(Type from, Type to), ITransition>();
            _defaultTransition = defaultTransition ?? new EmptyTransition();
        }

        public void RegisterTransition(Type from, Type to, ITransition transition)
        {
            transition ??= _defaultTransition;
            _transitions[(from, to)] = transition;
        }

        public ITransition Resolve(Type from, Type to)
        {
            if (_transitions.TryGetValue((from, to), out var transition))
            {
                return transition;
            }

            return _defaultTransition;
        }
    }
}