using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Attributes;

namespace WarWolfWorks.Utility.Coloring
{
    /// <summary>
    /// Color class used to apply colors to a <see cref="ColorManager"/>.
    /// </summary>
    [System.Serializable]
    public sealed class CMColor : IEquatable<CMColor>
    {
        [FormerlySerializedAs("value"), NoS, SerializeField]
        private Color s_Value;
        /// <summary>
        /// The color value this <see cref="CMColor"/> originally applies.
        /// </summary>
        public Color Value
        {
            get => s_Value;
            set => this.s_Value = value;
        }

        [FormerlySerializedAs("duration"), NoS, SerializeField]
        private float s_Duration;
        /// <summary>
        /// The duration at which this <see cref="CMColor"/> will stay inside a <see cref="ColorManager"/> based on <see cref="ColorBehavior"/>.
        /// (Serialized as "Duration" inside the inspector)
        /// </summary>
        public float MaxDuration => s_Duration;

        /// <summary>
        /// The current countdown of this <see cref="CMColor"/>. Used based on <see cref="ColorBehavior"/>.
        /// </summary>
        public float CurrentDuration { get; set; }

        [FormerlySerializedAs("behaviour"), NoS, SerializeField]
        private ColorBehavior s_Behavior;

        /// <summary>
        /// Determines how this <see cref="CMColor"/>'s countdown behaves.
        /// </summary>
        public ColorBehavior Behavior
        {
            get => s_Behavior;
            set => s_Behavior = value;
        }
        
        [FormerlySerializedAs("application"), NoS, SerializeField]
        private ColorApplication s_Application;
        /// <summary>
        /// Determines how the color is applied to a <see cref="ColorManager"/>.
        /// </summary>
        public ColorApplication Application
        {
            get => s_Application;
            set => s_Application = value;
        }

        /// <summary>
        /// Creates a <see cref="CMColor"/> instance.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="duration"></param>
        /// <param name="behaviour"></param>
        /// <param name="application"></param>
        public CMColor(Color color, float duration, ColorBehavior behaviour, ColorApplication application)
        {
            Value = color;
            this.s_Duration = duration;
            CurrentDuration = MaxDuration;
            Behavior = behaviour;
            Application = application;
        }

        /// <summary>
        /// The explicit returning of a <see cref="Behavior"/>.
        /// </summary>
        /// <param name="color"></param>
        public static explicit operator ColorBehavior(CMColor color) => color.Behavior;

        /// <summary>
        /// The explicit returning of a <see cref="Application"/>.
        /// </summary>
        /// <param name="color"></param>
        public static explicit operator ColorApplication(CMColor color) => color.Application;

        /// <summary>
        /// The implicit returning of a <see cref="Value"/>.
        /// </summary>
        /// <param name="color"></param>
        public static implicit operator Color(CMColor color) => color.Value;

        /// <summary>
        /// Returns true if both instances are equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(CMColor a, CMColor b)
            => a.Equals(b);

        /// <summary>
        /// Returns true if both instances are not equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(CMColor a, CMColor b)
            => !a.Equals(b);

        /// <summary>
        /// CMColor's Equals() method.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is CMColor color && Equals(color);
        }

        /// <summary>
        /// Returns true if all variables in the other <see cref="CMColor"/> are the same.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CMColor other)
        {
            return Value.Equals(other.Value) &&
                   MaxDuration == other.MaxDuration &&
                   Behavior == other.Behavior &&
                   Application == other.Application;
        }

        /// <summary>
        /// Returns this <see cref="CMColor"/>'s HashCode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = 102643408;
            hashCode = hashCode * -1521134295 + EqualityComparer<Color>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + MaxDuration.GetHashCode();
            hashCode = hashCode * -1521134295 + Behavior.GetHashCode();
            hashCode = hashCode * -1521134295 + Application.GetHashCode();
            return hashCode;
        }
    }
}