using System;
using System.Collections.Generic;
using System.Text;

namespace WarWolfWorks.Attributes
{
    /// <summary>
    /// What kind of argument a auto-generated command through <see cref="ConsoleMethodMakerAttribute"/> accepts.
    /// </summary>
    [Obsolete]
    internal enum ConsoleMethodMakerType
    {
        /// <summary>
        /// No argument is passed.
        /// </summary>
        Void = 0,
        /// <summary>
        /// <see cref="string"/> type.
        /// </summary>
        String = 1,
        /// <summary>
        /// <see cref="int"/> type.
        /// </summary>
        Int = 2,
        /// <summary>
        /// <see cref="float"/> type.
        /// </summary>
        Float = 3,
        /// <summary>
        /// <see cref="bool"/> type.
        /// </summary>
        Bool = 4
    }
}
