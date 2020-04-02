using System;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.NyuEntities;
using static WarWolfWorks.Constants;

namespace WarWolfWorks.Security
{
    /// <summary>
    /// Exception used for <see cref="NyuEntities"/>.
    /// </summary>
    internal sealed class NyuEntityException : Exception
    {
        private string ActMessage;

        public override string Message => ActMessage;

        /// <summary>
        /// <list type="Codes">
        /// <listheader>Codes:</listheader>
        /// <item>
        /// <term>1:</term>
        /// <description><see cref="Nyu.AddNyuComponent{T}"/> was called with an already existing T component.</description>
        /// </item>
        /// <item>
        /// <term>2:</term>
        /// <description>An entity was destroyed without using <see cref="NyuManager.Destroy(Nyu)"/>.</description>
        /// </item>
        /// <item>
        /// <term>3:</term>
        /// <description>An <see cref="Nyu"/> class implements <see cref="INyuOnEnable"/> and/or <see cref="INyuOnDisable"/>.</description>
        /// </item>
        /// <item>
        /// <term>4:</term>
        /// <description>A <see cref="INyuComponent"/> was incorrectly added.</description>
        /// </item>
        /// <item>
        /// <term>5:</term>
        /// <description>A <see cref="Nyu.AddNyuComponent(Type)"/> was attempted where the type given isn't implementing <see cref="INyuComponent"/>.</description>
        /// </item>
        /// <item>
        /// <term>6:</term>
        /// <description><see cref="NyuManager"/> was destroyed.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="code"></param>
        public NyuEntityException(int code) : base()
        {
            switch(code)
            {
                default:
                    ActMessage = "Unknown Error";
                    break;
                case 1:
                    ActMessage = "Attempted to add an already exisiting component through Entity.AddEntityComponent<T>.";
                    break;
                case 2:
                    ActMessage = "Entity was destroyed without using EntityManager.Destroy(Entity).";
                    break;
                case 3:
                    ActMessage = "Entity class implements INyuEnable and/or INyuDisable; Use Entity.OnEnabled or Entity.OnDisabled events instead.";
                    break;
                case 4:
                    ActMessage = "NyuComponent was added incorrectly.";
                    break;
                case 5:
                    ActMessage = "A Nyu.AddNyuComponent(Type) was attempted where the type given isn't implementing INyuComponent.";
                    break;
                case 6:
                    ActMessage = $"{VARN_NYUMANAGER} was destroyed.";
                    break;
            }
        }
    }
}
