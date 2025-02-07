namespace Damas.Combat
{
    /// Layla
    public class AttackCommand : ICommand
    {
        private readonly AttackStat clientAttack;
        private readonly HealthStat receiverHealth;

        public AttackCommand(AttackStat clientAttack, HealthStat receiverHealth)
        {
            this.clientAttack = clientAttack;
            this.receiverHealth = receiverHealth;
        }

        public bool WouldKill()
        {
            return clientAttack.CurrentValue > receiverHealth.CurrentValue;
        }

        public void Execute()
        {
            receiverHealth.ReceiveDamage(clientAttack.CurrentValue);
        }
    }
}