using System;

namespace WarWolfWorks.Security
{
    /// <summary>
    /// Used with <see cref="ProjectileException"/> for all unallowed operations.
    /// </summary>
    public sealed class ProjectileException : Exception
    {
        private string ActMessage;

        /// <summary>
        /// The message of the unallowed operation.
        /// </summary>
        public override string Message => ActMessage;

        /// <summary>
        /// Creates a new <see cref="ProjectileException"/>.
        /// </summary>
        /// <param name="type"></param>
        public ProjectileException(ProjectileExceptionType type) : base()
        {
            switch(type)
            {
                case ProjectileExceptionType.UnpopulatedProjectileNew:
                    ActMessage = "A Projectile.New call was attempted when a Projectile.Populate method was never called.";
                    break;
                case ProjectileExceptionType.PopulationLessThanOne:
                    ActMessage = "Projectile.Populate was called with population given at less than one.";
                    break;
                case ProjectileExceptionType.InstantiatedWithoutPostNew:
                    ActMessage = "A Projectile.New call was attempted when Projectile.PostNew method was not called at the end of it.";
                    break;
            }
        }
    }
}
