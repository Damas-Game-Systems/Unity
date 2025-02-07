using UnityEngine;

namespace Damas.Combat
{
    /// Layla
    public class HealthStat
    {
        private readonly int maxValue;

        private int currentValue;

        /// <summary>
        /// The current value of this stat.
        /// </summary>
        public int CurrentValue { get { return currentValue; } }

        public HealthStat(int maxValue)
        {
            this.maxValue = maxValue;
            Reset();
        }

        /// <summary>
        /// Resets the value of the stat to the one used
        /// during construction.
        /// </summary>
        public void Reset()
        {
            currentValue = maxValue;
        }

        public void ReceiveDamage(int amount)
        {
            currentValue -= amount;

            if (currentValue < 0)
            {
            }
        }

        public void ReceiveHeal(int amount)
        {
            currentValue += amount;

            if (currentValue > maxValue)
            {
                currentValue = maxValue;
            }
        }
    }
}