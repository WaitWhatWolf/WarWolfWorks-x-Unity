using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.MovementSystem
{
    /// <summary>
    /// To be used with <see cref="NyuMovement"/> to designate a velocity.
    /// </summary>
    [System.Serializable]
    public sealed class Velocity
    {
        /// <summary>
        /// Value of the velocity.
        /// </summary>
        public Vector3 Value;
        /// <summary>
        /// When the velocity timer reaches 0, it will be removed.
        /// </summary>
        public bool DeleteOnCount0;
        /// <summary>
        /// Affections. See <see cref="INyuStat"/> for more info.
        /// </summary>
        public int[] Affections;

        [SerializeField]
        private float s_StartTime;
        /// <summary>
        /// Time which was set for this velocity.
        /// </summary>
        public float StartTime { get => s_StartTime; set => s_StartTime = value; }
        /// <summary>
        /// Current countdown time.
        /// </summary>
        public float Time { get; set; }

        /// <summary>
        /// Returns the percent of the velocity runout. (Time / StartTime).
        /// </summary>
        public float Time01 => Time / StartTime;

        internal bool UsesStat;

        /// <summary>
        /// Creates a simple velocity.
        /// </summary>
        /// <param name="value"></param>
        public Velocity(Vector3 value)
        {
            Value = value;
            DeleteOnCount0 = false;
            StartTime = Time = 0;
            UsesStat = false;
            Affections = null;
        }

        /// <summary>
        /// Creates a velocity with a countdown.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <param name="deleteOn0"></param>
        public Velocity(Vector3 value, float time, bool deleteOn0)
        {
            Value = value;
            DeleteOnCount0 = deleteOn0;
            StartTime = Time = time;
            UsesStat = false;
            Affections = null;
        }

        /// <summary>
        /// Creates a velocity with affections.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="affections"></param>
        public Velocity(Vector3 value, params int[] affections)
        {
            Value = value;
            DeleteOnCount0 = false;
            UsesStat = true;
            StartTime = Time = 0;
            Affections = affections;
        }

        /// <summary>
        /// Creates a complete velocity.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <param name="deleteOn0"></param>
        /// <param name="affections"></param>
        public Velocity(Vector3 value, float time, bool deleteOn0, params int[] affections)
        {
            Value = value;
            DeleteOnCount0 = deleteOn0;
            StartTime = Time = time;
            UsesStat = true;
            Affections = affections;
        }

        /// <summary>
        /// Creates a duplicate of the given velocity.
        /// </summary>
        /// <param name="original"></param>
        public Velocity(Velocity original)
        {
            Value = original.Value;
            DeleteOnCount0 = original.DeleteOnCount0;
            StartTime = original.StartTime;
            Time = original.Time;
            UsesStat = original.UsesStat;
            Affections = original.Affections;
        }
    }
}
