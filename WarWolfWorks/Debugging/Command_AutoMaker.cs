using System;
using System.Collections.Generic;
using System.Text;
using WarWolfWorks.Attributes;

namespace WarWolfWorks.Debugging
{
    [Obsolete]
    internal sealed class Command_AutoMaker : Command
    {
        public override string Recognition => pv_Recognition;

        public override string ArgumentHelper => pv_ArgumentHelper;

        public override void OnPassed(string arg)
        {
            try
            {
                switch (pv_Type)
                {
                    case ConsoleMethodMakerType.String:
                        pv_Action(arg);
                        break;
                    case ConsoleMethodMakerType.Int:
                        int number = Convert.ToInt32(arg);
                        pv_Action(number);
                        break;
                    case ConsoleMethodMakerType.Float:
                        float singleNumber = Convert.ToSingle(arg);
                        pv_Action(singleNumber);
                        break;
                    case ConsoleMethodMakerType.Bool:
                        bool boolval = Convert.ToBoolean(arg);
                        pv_Action(boolval);
                        break;
                }
            }
            catch (Exception e) { AdvancedDebug.LogError(e); }
        }

        private Action<object> pv_Action;
        private string pv_Recognition;
        private string pv_Description;
        private string pv_ArgumentHelper;
        private ConsoleMethodMakerType pv_Type;

        public Command_AutoMaker(Action<object> action, ConsoleMethodMakerType type, string recognition, string descripiton, string argumentHelper)
        {
            pv_Action = action;
            pv_Type = type;
            pv_Recognition = recognition;
            pv_Description = descripiton;
            pv_ArgumentHelper = argumentHelper;
        }
    }
}
