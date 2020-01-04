using System;
using WarWolfWorks.Internal;

namespace WarWolfWorks.Maps
{
    internal sealed class StartingMapIncorrectLocationException : WWWException
    {
        public StartingMapIncorrectLocationException() : 
            base("Cannot load the starting map as the file path given was incorrent or was empty.")
        {

        }
    }
}
