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
        /// <see cref="Projectile"/>.New was called when <see cref="Projectile.Populate{T}(int, Action{T})"/> was never called.
        /// </summary>
        UnpopulatedProjectileNew,
        /// <summary>
        /// <see cref="Projectile.Populate{T}(int, Action{T})"/> was given a population lesser than one.
        /// </summary>
        PopulationLessThanOne,
        /// <summary>
        /// <see cref="Projectile"/>.New was called when <see cref="Projectile.PostNew(Projectile)"/> was not called at the end of it.
        /// </summary>
        InstantiatedWithoutPostNew
    }
}
