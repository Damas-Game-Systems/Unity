using System;

namespace Damas.Combat
{
    /// Layla
    public class HealthStat
    {
        private readonly int maxValue;

        private int currentValue;
        public event Action OnHealthChanged;

        /// <summary>
        /// The current value of this stat.
        /// </summary>
        public int CurrentValue { get { return currentValue; } }
        public int MaxValue { get { return maxValue; } }

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
                currentValue = 0;
            }
            OnHealthChanged?.Invoke();
        }

        public void ReceiveHeal(int amount)
        {
            currentValue += amount;

            if (currentValue > maxValue)
            {
                currentValue = maxValue;
            }
            OnHealthChanged?.Invoke();
        }
    }
}