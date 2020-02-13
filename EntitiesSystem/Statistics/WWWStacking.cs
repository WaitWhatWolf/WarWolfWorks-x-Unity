using System.Collections.Generic;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EntitiesSystem.Statistics
{
    /// <summary>
    /// Default stacking which is used if <see cref="Stats.Stacking"/> is not set.
    /// </summary>
    public class WWWStacking : Stacking
    {
        /// <summary>
        /// Stacking index: Used as base value.
        /// </summary>
        public const int Original = 0;
        /// <summary>
        /// Stacking index: All stats tagged will be added onto the original value.
        /// </summary>
        public const int Adder = 1;
        /// <summary>
        /// Stacking index: All stats tagged will stack on themselves to form a final value, which will then multiply the original.
        /// </summary>
        public const int StackingOriginalMultiplier = 2;
        /// <summary>
        /// Stacking index: tagged will muliply the original value and Adder value. (Applies before StackingOriginalMultiplier)
        /// </summary>
        public const int StackingMultiplier = 3;
        /// <summary>
        /// Stacking index: Multiplies the original immediatly without stacking.
        /// </summary>
        public const int OriginalMultiplier = 4;
        /// <summary>
        /// Stacking index: Multiplies the original with adder values immediatly without stacking.
        /// </summary>
        public const int Multiplier = 5;
        /// <summary>
        /// Stacking index: All stats tagged will stack on themselves to form a final value, which will then multiply the total value after all multiplications (excluding FullMultiplier)
        /// </summary>
        public const int StackingFullMultiplier = 6;
        /// <summary>
        /// Stacking index: Absolute multiplier.
        /// </summary>
        public const int FullMultiplier = 7;

        /// <summary>
        /// Returns the calculated value. See <see cref="WWWStacking"/>'s constant values for more info.
        /// </summary>
        /// <param name="BaseStat"></param>
        /// <returns></returns>
        public override float CalculatedValue(IStat BaseStat)
        {
            float GetMultiplierValue(float original, ICollection<float> values)
            {
                float saved = 0;
                foreach (float f in values)
                {
                    saved += f * original;
                }

                return saved;
            }

            IStat[] toUse = GetStatsByAffections(BaseStat.Affections);

            float adder = Hooks.EnumerableConcat(BaseStat.Value, GetStatValuesByStacking(toUse, Adder)) - BaseStat.Value;
            float stackOrigMul = Hooks.EnumerableConcat(1, GetStatValuesByStacking(toUse, StackingOriginalMultiplier));
            float stackMul = Hooks.EnumerableConcat(1, GetStatValuesByStacking(toUse, StackingMultiplier));

            float origMulAdd = GetMultiplierValue(BaseStat.Value, GetStatValuesByStacking(toUse, OriginalMultiplier));
            float MulAdd = GetMultiplierValue(adder, GetStatValuesByStacking(toUse, Multiplier));

            float added = ((adder * stackMul) + origMulAdd + (BaseStat.Value * stackOrigMul)) + MulAdd;
            //AdvancedDebug.Log($"Added value is {added}; Based on adder: {adder}, origMulAdd: {origMulAdd}, BaseStat: {BaseStat}, stackOrigMul: {stackOrigMul}, MulAdd: {MulAdd}, stackMul: {stackMul}");

            float stackFullMul = Hooks.EnumerableConcat(1, GetStatValuesByStacking(toUse, StackingFullMultiplier));
            float fullMulAdd = GetMultiplierValue(added, GetStatValuesByStacking(toUse, FullMultiplier));

            float toReturn = (added * stackFullMul) + fullMulAdd;

            return toReturn;
        }
    }
}
