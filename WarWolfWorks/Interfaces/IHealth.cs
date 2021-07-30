using System;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Basic interface for implementing a Health class.
    /// </summary>
	public interface IHealth
	{
        /// <summary>
        /// Max value which <see cref="CurrentHealth"/> can reach.
        /// </summary>
        float MaxHealth { get; }
        /// <summary>
        /// The current health.
        /// </summary>
        float CurrentHealth { get; }
        /// <summary>
        /// Attempts to add the given amount to the current health.
        /// </summary>
        /// <param name="amount">Amount used.</param>
        /// <param name="added">
        /// The added amount of health; 
        /// As an example, if <see cref="CurrentHealth"/> is at 980 and <see cref="MaxHealth"/> is at 1000,
        /// but the amount given is 100, the amount added will be 20 instead of 100.
        /// </param>
        /// <returns>True if addition was successful.</returns>
        bool AddHealth(float amount, out float added);
        /// <summary>
        /// Attempts to remove the given amount from <see cref="CurrentHealth"/>.
        /// </summary>
        /// <param name="amount">Amount used.</param>
        /// <param name="removed">
        /// The removed amount of health; As an example, 
        /// if <see cref="CurrentHealth"/> is at 100 but amount given is 120, the amount removed will be 100 instead of 120.
        /// </param>
        /// <returns>True if removal was successful.</returns>
        bool RemoveHealth(float amount, out float removed);
	}
}