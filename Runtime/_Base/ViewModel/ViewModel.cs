namespace Yans.ViewModels
{
    public abstract class ViewModel
    {
        protected ViewModel() { }

        #region public methods

        internal void Create()
        {
            OnCreated();
        }

        internal void Abort()
        {
            OnAborted();
        }

        #endregion

        #region protected methods
        protected virtual void OnCreated() { }

        protected virtual void OnAborted() { }
        #endregion
    }
}