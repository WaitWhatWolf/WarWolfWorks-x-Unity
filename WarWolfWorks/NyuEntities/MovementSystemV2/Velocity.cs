using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.NyuEntities.Statistics;

namespace WarWolfWorks.NyuEntities.MovementSystemV2
{
    /// <summary>
    /// A basic <see cref="IVelocity"/> which can either take a simple Vector3 value or a Vector3 value with affections.
    /// </summary>
    [System.Serializable]
    public class Velocity : IVelocity
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Nyu NyuMain => Parent.NyuMain;

        private Vector3 Value;
        private Stat ValueX = new Stat(0, 0, 0);
        private Stat ValueY = new Stat(0, 0, 0);
        private Stat ValueZ = new Stat(0, 0, 0);

        private int[] Affections;
        private bool UsesAffections;

        /// <summary>
        /// Is this velocity currently residing in a NyuMovement?
        /// </summary>
        public bool Initiated { get; private set; }

        /// <summary>
        /// The parent of this velocity.
        /// </summary>
        public NyuMovement Parent { get; private set; }

        /// <summary>
        /// Retrieves the Vector3 value that will be applied.
        /// </summary>
        public Vector3 GetValue() => UsesAffections 
            ? new Vector3(
            NyuMain.Stats.CalculatedValue(ValueX), 
            NyuMain.Stats.CalculatedValue(ValueY),
            NyuMain.Stats.CalculatedValue(ValueZ)) 
            : Value;

        /// <summary>
        /// Returns the Vector3 value unaffected by affections.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetUnaffectedValue() => Value;

        /// <summary>
        /// Sets the value of the base vector.
        /// </summary>
        public void SetValue(Vector3 value)
        {
            Value = value;
            ValueX.Value = value.x;
            ValueY.Value = value.y;
            ValueZ.Value = value.z;
        }

        /// <summary>
        /// Returns the affections applied to this velocity.
        /// </summary>
        public int[] GetAffections() => Affections;

        /// <summary>
        /// Sets the affections affecting this velocity.
        /// </summary>
        public void SetAffections(int[] affections)
        {
            Affections = affections;
            ValueX.Affections = affections;
            ValueY.Affections = affections;
            ValueZ.Affections = affections;

            UsesAffections = Affections != null;
        }

        /// <summary>
        /// Initiates this velocity.
        /// </summary>
        public bool Init(NyuMovement parent)
        {
            if (Initiated || parent == null)
                return Initiated;

            Parent = parent;
            return Initiated = true;
        }


        /// <summary>
        /// Creates a simple velocity.
        /// </summary>
        /// <param name="value"></param>
        public Velocity(Vector3 value)
        {
            SetValue(value);
            SetAffections(null);
        }

        /// <summary>
        /// Creates a velocity with affections.
        /// </summary>
        public Velocity(Vector3 value, params int[] affections)
        {
            SetValue(value);
            SetAffections(affections);
        }

        /// <summary>
        /// Creates a duplicate of the given velocity.
        /// </summary>
        public Velocity(Velocity original)
        {
            SetValue(original.Value);
            SetAffections(original.Affections);
        }
    }
}
