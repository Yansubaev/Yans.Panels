namespace Yans.UI.UIScreens
{
    /// <summary>
    /// Interface for screen result listeners.
    /// </summary>
    public interface IScreenResultListener
    {
        /// <summary>
        /// Called when a screen result is received.
        /// </summary>
        /// <param name="result">The result of the screen.</param>
        void OnScreenResult(UIScreen sender, ScreenResult result);
    }
}