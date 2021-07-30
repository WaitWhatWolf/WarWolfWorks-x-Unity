using System;
using UnityEngine;
using WarWolfWorks.Attributes;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Debugging
{
    [DeveloperCommand]
    internal sealed class Command_Resources_Load : Command
    {
        public override string Recognition { get; } = CONVAR_STREAMING + "RESOURCES_LOAD";

        public override string ArgumentHelper { get; } = "<string> <float> <float> <float>";

        public override string Description { get; } = "Loads an object from resources based on given path and position.";

        public override void OnPassed(string arg)
        {
            try
            {
                string[] args = arg.Split(' ');
                GameObject toLoad = Resources.Load<GameObject>(args[0]);
                Vector3 pos = new Vector3(Convert.ToSingle(args[1]), Convert.ToSingle(args[2]), Convert.ToSingle(args[3]));
                UnityEngine.Object.Instantiate(toLoad, pos, Quaternion.identity);
            } catch (Exception e) { e.LogException(); }
        }
    }
}
