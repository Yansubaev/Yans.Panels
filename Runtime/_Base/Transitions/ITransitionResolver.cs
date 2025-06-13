using System;

namespace Yans.UI.Transitions
{
    public interface ITransitionResolver
    {
        ITransition Resolve(Type from, Type to);
    }
}
