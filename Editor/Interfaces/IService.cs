using WarWolfWorks.EditorBase.Services;

namespace WarWolfWorks.EditorBase.Interfaces
{
    /// <summary>
    /// Used with <see cref="ServicesWindow"/> to draw an editor window.
    /// </summary>
    internal interface IService
    {
        /// <summary>
        /// Name to be displayed on the tab of the <see cref="ServicesWindow"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Invoked when <see cref="ServicesWindow"/> is enabled.
        /// </summary>
        void OnEnable();
        /// <summary>
        /// Invoked when <see cref="ServicesWindow"/> is disabled.
        /// </summary>
        void OnDisable();
        /// <summary>
        /// Invoked every frame when <see cref="ServicesWindow"/> focuses this window.
        /// </summary>
        void Draw();
    }
}
