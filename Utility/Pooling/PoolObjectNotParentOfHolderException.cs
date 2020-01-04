using System;

namespace WarWolfWorks.Utility.Pooling
{
    internal class PoolObjectNotParentOfHolderException : Exception
    {
        public override string Message => "One or more objects inside the CustomPool is not parent of it's holder. Make sure you are setting all of their parents to it's Holder.";
    }
}
