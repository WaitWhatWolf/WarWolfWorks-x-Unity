using System;
using WarWolfWorks.NyuEntities.ProjectileSystem;

namespace WarWolfWorks.Security
{
    /// <summary>
    /// Exception class used by <see cref="NyuEntities.ProjectileSystem"/>.
    /// </summary>
    internal sealed class NyuProjectileException : Exception
    {
        public override string Message => pv_Message;

        /// <summary>
        /// <list type="bullet">
        /// <item>
        /// <term>0:</term>
        /// <description>Unknown or undescribed exception.</description>
        /// </item>
        /// <item>
        /// <term>1:</term>
        /// <description>Projectile Manager's Init(int) was given a value of less than 1.</description>
        /// </item>
        /// <item>
        /// <term>2:</term>
        /// <description>Projectile Manager's New or End method was called without being initiated.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="code"></param>
        public NyuProjectileException(int code) : base()
        {
            switch(code)
            {
                case 0:
                    pv_Message = "Unknown Exception";
                    break;
                case 1:
                    pv_Message = "Projectile Manager's Init(int) was given a value of less than 1.";
                    break;
                case 2:
                    pv_Message = "Projectile Manager's New or End method was called without being initiated.";
                    break;
            }
        }

        private string pv_Message;
    }
}
