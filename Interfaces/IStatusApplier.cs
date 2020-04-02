using System;
using System.Collections.Generic;
using WarWolfWorks.EntitiesSystem.Statusing;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// The interface used as implementation for <see cref="IStatus"/> handling. (Inherits from <see cref="IEntity"/>)
    /// </summary>
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public interface IStatusApplier : IEntity
    {
        /// <summary>
        /// Returns a list for all <see cref="IStatus"/>es affecting this <see cref="IStatusApplier"/>.
        /// </summary>
        IEnumerable<IStatus> GetAllStatuses { get; }
        /// <summary>
        ///  Returns a list for all <see cref="Resistance"/>s affecting this <see cref="IStatusApplier"/>.
        /// </summary>
        IEnumerable<Resistance> GetAllResistances { get; }
        /// <summary>
        /// Returns true when a <see cref="Resistance"/>'s <see cref="Resistance.ResistantTo"/> is the same as type given.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        bool ContainsResistance(Type of);
        /// <summary>
        /// Returns a <see cref="Resistance"/> based on <see cref="Resistance.ResistantTo"/>.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        Resistance GetResistance(Type of);
        /// <summary>
        /// Adds an <see cref="IStatus"/> to the list of <see cref="IStatus"/>es affecting this <see cref="IStatusApplier"/>.
        /// It also triggers <see cref="IStatus.OnStart(IStatusApplier)"/> based on <see cref="IStatus.OverlapType"/>.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        bool AddStatus(IStatus status);
        /// <summary>
        /// Does the same as <see cref="AddStatus(IStatus)"/> for a params of <see cref="IStatus"/>es.
        /// </summary>
        /// <param name="statuses"></param>
        void AddStatuses(params IStatus[] statuses);
        /// <summary>
        /// Adds a <see cref="Resistance"/> to the list of <see cref="Resistance"/>s affecting this <see cref="IStatusApplier"/>.
        /// </summary>
        /// <param name="resistance"></param>
        void AddResistance(Resistance resistance);
        /// <summary>
        /// Does the same as <see cref="AddResistances(Resistance[])"/> for a params of <see cref="IStatus"/>es.
        /// </summary>
        /// <param name="resistances"></param>
        void AddResistances(params Resistance[] resistances);
        /// <summary>
        /// Removes a <see cref="IStatus"/> from the list of affecting statuses.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ignoreOnEnd">If true, <see cref="IStatus.OnEnd(IStatusApplier)"/> will be ignored.</param>
        /// <returns></returns>
        bool RemoveStatus(IStatus status, bool ignoreOnEnd);
        /// <summary>
        /// Does the same as <see cref="RemoveStatus(IStatus, bool)"/> for a params of <see cref="IStatus"/>es.
        /// </summary>
        /// <param name="ignoreOnEnd"></param>
        /// <param name="statuses"></param>
        void RemoveStatuses(bool ignoreOnEnd, params IStatus[] statuses);
        /// <summary>
        /// Removes a <see cref="Resistance"/> from the list of affecting <see cref="Resistance"/>s.
        /// </summary>
        /// <param name="resistance"></param>
        /// <returns></returns>
        bool RemoveResistance(Resistance resistance);
        /// <summary>
        /// Same as <see cref="RemoveStatuses(bool, IStatus[])"/> for a params of <see cref="Resistance"/>s.
        /// </summary>
        /// <param name="resistances"></param>
        void RemoveResistances(params Resistance[] resistances);
    }
}
