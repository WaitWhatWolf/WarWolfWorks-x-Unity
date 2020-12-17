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
        /// <list type="number">
        /// <item>
        /// <term>1:</term>
        /// <see cref="NyuProjectileManager.Initiate(int)"/> was given a value less than 1.
        /// </item>
        /// <item>
        /// <term>2:</term>
        /// <see cref="NyuProjectileManager.Initiate(int)"/> was called when it's singleton instance was null.
        /// </item>
        /// <item>
        /// <term>3:</term>
        /// NyuProjectileManager.New or NyuProjectileManager.End was called when it's singleton instance was null.
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
                    pv_Message = "NyuProjectileManager.Initiate<TProjectile>(int) was given a value less than 1.";
                    break;
                case 2:
                    pv_Message = "NyuProjectileManager.Initiate<TProjectile>(int) was called when it's singleton instance was null; Make sure to " +
                        "add a GameObject to your scene with a ProjectileManager attached to it.";
                    break;
                case 3:
                    pv_Message = "NyuProjectileManager.New or NyuProjectileManager.End was called when it's singleton instance was null.";
                    break;
            }
        }

        private string pv_Message;
    }
}
