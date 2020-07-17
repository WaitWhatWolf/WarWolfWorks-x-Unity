#pragma warning disable CS1591

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// An item which has instructions when duplicated.
    /// </summary>
    public interface ICloneInstructable
	{
        object[] GetInstructions();
        void SetInstructions(object[] instructions);
	}
}