using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WarWolfWorks.Internal;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Struct which returns a string based on language given; If used directly, it will use <see cref="Application.systemLanguage"/> to determine the language.
    /// To use with a specified language, use an indexer.
    /// (Example: ItemName = LanguageItemName[<see cref="SystemLanguage.Polish"/>])
    /// </summary>
    public struct LanguageString
    {
        /// <summary>
        /// Returned by default.
        /// </summary>
        public string English;

        /// <summary>
        /// All values contained by this <see cref="LanguageString"/>.
        /// </summary>
        public (string, SystemLanguage)[] Values;

        /// <summary>
        /// Creates a new <see cref="LanguageString"/>.
        /// </summary>
        /// <param name="english"></param>
        /// <param name="other"></param>
        public LanguageString(string english, params (string, SystemLanguage)[] other)
        {
            Values = other;
            English = english;
        }

        /// <summary>
        /// Returns a string associated with this language; If not found, will return <see cref="English"/> as default.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public string this[SystemLanguage language]
        {
            get
            {
                if (language == SystemLanguage.English)
                    return English;

                int index = Array.FindIndex(Values, v => v.Item2 == language);

                if (index == -1)
                    return English;

                return Values[index].Item1;
            }
        }

        /// <summary>
        /// Returns the text instead of the full type.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this[Settings.LibraryLanguage];
        }

        /// <summary>
        /// Pointer to <see cref="LanguageString"/>[<see cref="Settings.LibraryLanguage"/>].
        /// </summary>
        /// <param name="languageString"></param>
        public static implicit operator string(LanguageString languageString)
            => languageString[Settings.LibraryLanguage];
    }
}
