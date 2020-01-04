using System;

namespace WarWolfWorks.Interfaces
{
	public interface IHealth
	{
        event Action<IHealth> OnDeath;
        float MaxHealth { get; set; }
        float CurrentHealth { get; }
        void AddHealth(float amount);
        void RemoveHealth(float amount);
	}
}