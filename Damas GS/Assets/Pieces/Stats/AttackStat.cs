using UnityEngine;

namespace Damas.Combat
{
    /// Layla
    public class AttackStat
    {
        private readonly int defaultValue;

        private int currentValue;

        /// <summary>
        /// The current value of this stat.
        /// </summary>
        public int CurrentValue { get { return currentValue; } }

        public AttackStat(int defaultValue)
        {
            this.defaultValue = defaultValue;
            Reset();
        }

        /// <summary>
        /// Resets the value of the stat to the one used
        /// during construction.
        /// </summary>
        public void Reset()
        {
            currentValue = defaultValue;
        }

        /// <summary>
        /// I don't even remember if we're doing this,
        /// but its here if we need it.
        /// </summary>
        /// 
        /// <param name="amount">
        /// The amount to modify the stat by
        /// </param>
        public void ModifyBy(int amount)
        {
            int newValue = currentValue + amount;

            /// Attack can't be negative
            currentValue = (newValue < 0)
                ? 0
                : newValue;
        }
    }
}