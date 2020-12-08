using UnityEngine;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Used by <see cref="LanguageString"/>.
    /// </summary>
    public struct Langroup
    {
        /// <summary>
        /// Value of this group in string.
        /// </summary>
        public string Value;
        /// <summary>
        /// What language is this value used for.
        /// </summary>
        public SystemLanguage Language;

        /// <summary>
        /// Creates a new <see cref="Langroup"/> instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="language"></param>
        public Langroup(string value, SystemLanguage language)
        {
            Value = value;
            Language = language;
        }

        /// <summary>
        /// Implicitly converts a tuple into a Langroup.
        /// </summary>
        /// <param name="tuple"></param>
        public static implicit operator Langroup((string, SystemLanguage) tuple)
        {
            return new Langroup(tuple.Item1, tuple.Item2);
        }
    }
}
