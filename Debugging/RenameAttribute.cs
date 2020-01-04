namespace WarWolfWorks.Debugging
{
    using UnityEngine;

    /// <summary>
    /// Renames a variable in the inspector without changing it in code or creating a custom editor.
    /// </summary>
    public class RenameAttribute : PropertyAttribute
    {
        /// <summary>
        /// Name that it will be changed to.
        /// </summary>
        public string label;

        /// <summary>
        /// Renames a variable in the inspector without changing it in code or creating a custom editor.
        /// </summary>
        /// <param name="label">Name that it will be changed to.</param>
        public RenameAttribute(string label)
        {
            this.label = label;
        }
    }
}