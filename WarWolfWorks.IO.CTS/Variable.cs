using System;

namespace WarWolfWorks.IO.CTS
{
    /// <summary>
    /// Used by <see cref="Catalog"/> 
    /// </summary>
    public sealed class Variable : IEquatable<string>
    {
        private string h_Name;
        private string h_Value;

        /// <summary>
        /// The name of the variable.
        /// </summary>
        public string Name
        {
            get => h_Name;
            set
            {
                h_Name = value;
                UpdateFinalValue();
            }
        }

        /// <summary>
        /// The value of the variable.
        /// </summary>
        public string Value
        {
            get => h_Value;
            set
            {
                h_Value = value;
                UpdateFinalValue();
            }
        }

        private string FinalValue;

        private void UpdateFinalValue()
        {
            FinalValue = h_Name + Catalog.SV_VARIABLE_POINTER + h_Value;
        }

        /// <summary>
        /// Creates a new variable.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Variable(string name, string value)
        {
            h_Name = name;
            h_Value = value;

            UpdateFinalValue();
        }

        /// <summary>
        /// Returns the entire line of a variable in a <see cref="Catalog"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FinalValue;
        }

        /// <summary>
        /// Returns true if this variable is equal to the given string.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(string other)
        {
            return FinalValue == other;
        }
    }
}
