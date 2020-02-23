using System;

namespace WarWolfWorks.Security
{
    /// <summary>
    /// Exception used by the entity system.
    /// </summary>
    internal sealed class EntityException : Exception
    {
        #region Constants
        internal const string MESSAGE_WARNING_NOT_INITIATED_PROPERLY = "When instantiating an entity, avoid using Unity's instantiate method; Use EntityManager.New instead.";
        private const string MESSAGE_ERROR_TYPE_INVALID = "The given type is not an inherited type of Entity, Aborting...";
        private const string MESSAGE_ERROR_PREFAB_WITHOUT_ENTITY = "No entity was found on the given GameObject! Aborting...";
        private const string MESSAGE_ERROR_ADDCOMPONENT_TYPE_INVALID = "The given type for Entity.AddEntityComponent(Type) does not implement IEntityComponent! Aborting...";
        #endregion

        private string CurrentString;
        public override string Message => CurrentString;

        public EntityException(EntityExceptionType exceptionType) : base()
        {
            switch(exceptionType)
            {
                case EntityExceptionType.ENTITY_TYPE_NULL:
                    CurrentString = MESSAGE_ERROR_TYPE_INVALID;
                    break;
                case EntityExceptionType.GAMEOBJECT_PREFAB_NO_ENTITY:
                    CurrentString = MESSAGE_ERROR_PREFAB_WITHOUT_ENTITY;
                    break;
                case EntityExceptionType.ENTITY_COMPONENT_TYPE_INVALID:
                    CurrentString = MESSAGE_ERROR_ADDCOMPONENT_TYPE_INVALID;
                    break;
            }
        }
    }
}
