using System;
using System.Collections.Generic;
using UnityEngine;

namespace WarWolfWorks.Utility.Coloring
{
    /// <summary>
    /// Color struct used to apply colors to a <see cref="ColorManager"/>.
    /// </summary>
    [System.Serializable]
    public sealed class CMColor : IEquatable<CMColor>
    {
        [SerializeField]
        private Color value;
        /// <summary>
        /// The color value this <see cref="CMColor"/> originally applies.
        /// </summary>
        public Color Value
        {
            get => value;
            set => this.value = value;
        }

        [SerializeField]
        private float duration;
        /// <summary>
        /// The duration at which this <see cref="CMColor"/> will stay inside a <see cref="ColorManager"/> based on <see cref="ColorBehaviour"/>.
        /// (Serialized as "Duration" inside the inspector)
        /// </summary>
        public float MaxDuration => duration;

        /// <summary>
        /// The current countdown of this <see cref="CMColor"/>. Used based on <see cref="ColorBehaviour"/>.
        /// </summary>
        public float CurrentDuration { get; set; }

        [SerializeField]
        private ColorBehaviour behaviour;

        /// <summary>
        /// Determines how this <see cref="CMColor"/>'s countdown behaves.
        /// </summary>
        public ColorBehaviour Behaviour
        {
            get => behaviour;
            set => behaviour = value;
        }
        
        [SerializeField]
        private ColorApplication application;
        /// <summary>
        /// Determines how the color is applied to a <see cref="ColorManager"/>.
        /// </summary>
        public ColorApplication Application
        {
            get => application;
            set => application = value;
        }

        /// <summary>
        /// Creates a <see cref="CMColor"/> instance.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="duration"></param>
        /// <param name="behaviour"></param>
        /// <param name="application"></param>
        public CMColor(Color color, float duration, ColorBehaviour behaviour, ColorApplication application)
        {
            Value = color;
            this.duration = duration;
            CurrentDuration = MaxDuration;
            Behaviour = behaviour;
            Application = application;
        }

        /// <summary>
        /// The explicit returning of a <see cref="Behaviour"/>.
        /// </summary>
        /// <param name="color"></param>
        public static explicit operator ColorBehaviour(CMColor color) => color.Behaviour;

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
                   Behaviour == other.Behaviour &&
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
            hashCode = hashCode * -1521134295 + Behaviour.GetHashCode();
            hashCode = hashCode * -1521134295 + Application.GetHashCode();
            return hashCode;
        }
    }
}