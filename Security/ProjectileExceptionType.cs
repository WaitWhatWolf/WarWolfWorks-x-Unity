using System;
using WarWolfWorks.EntitiesSystem.Projectiles;

namespace WarWolfWorks.Security
{
    /// <summary>
    /// Used with <see cref="ProjectileException"/>.
    /// </summary>
    public enum ProjectileExceptionType
    {
        /// <summary>
        /// Projectile.New was called when Projectile.Populate{T}(int, Action{T}) was never called.
        /// </summary>
        UnpopulatedProjectileNew,
        /// <summary>
        /// Projectile.Populate{T}(int, Action{T}) was given a population lesser than one.
        /// </summary>
        PopulationLessThanOne,
        /// <summary>
        /// Projectile.New was called when Projectile.PostNew(Projectile) was not called at the end of it.
        /// </summary>
        InstantiatedWithoutPostNew
    }
}
