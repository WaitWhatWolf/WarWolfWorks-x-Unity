using System;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Basic interface for implementing a Health class.
    /// </summary>
	public interface IHealth
	{
        /// <summary>
        /// Invoked when <see cref="CurrentHealth"/> reaches 0.
        /// </summary>
        event Action<IHealth> OnDeath;
        /// <summary>
        /// Invoked when <see cref="IHealth.AddHealth(float)"/> is invoked. Float value is the health added.
        /// </summary>
        event Action<IHealth, float> OnHealthAdded;
        /// <summary>
        /// Value that clamps <see cref="CurrentHealth"/>.
        /// </summary>
        float MaxHealth { get; set; }
        /// <summary>
        /// The current health.
        /// </summary>
        float CurrentHealth { get; }
        /// <summary>
        /// Adds to <see cref="CurrentHealth"/>.
        /// </summary>
        /// <param name="amount"></param>
        void AddHealth(float amount);
        /// <summary>
        /// Removes from <see cref="CurrentHealth"/>.
        /// </summary>
        /// <param name="amount"></param>
        void RemoveHealth(float amount);
	}
}