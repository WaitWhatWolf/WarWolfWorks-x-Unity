using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using WarWolfWorks.Attributes;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Debugging
{
    [DeveloperCommand]
    internal sealed class Command_Object_Named_Move : Command
    {
        public override string Recognition { get; } = CONVAR_INGAME + "OBJECT_NAMED_MOVE";

        public override string ArgumentHelper { get; } = "<string> <float> <float> <float>";

        public override string Description { get; } = "Moves the first object with the given name to the given x y z position.";

        public override void OnPassed(string arg)
        {
            try
            {
                string[] args = arg.Split(' ');

                Vector3 pos = new Vector3(Convert.ToSingle(args[1]), Convert.ToSingle(args[2]), Convert.ToSingle(args[3]));
                GameObject[] objects = SceneManager.GetActiveScene().GetRootGameObjects();
                GameObject toMove = Array.Find(objects, obj => obj.name.ToUpper() == args[0].ToUpper());
                toMove.transform.position += pos;
            } catch (Exception e) { e.LogException(); }
        }
    }
}
