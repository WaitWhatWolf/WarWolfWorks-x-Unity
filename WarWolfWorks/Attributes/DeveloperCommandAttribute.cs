using System;
using System.Collections.Generic;
using System.Text;

namespace WarWolfWorks.Attributes
{
    /// <summary>
    /// Attribute used on <see cref="Command"/> classes which can only be used when <see cref="Console.DeveloperMode"/> is true.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DeveloperCommandAttribute : Attribute
    {
    }
}
