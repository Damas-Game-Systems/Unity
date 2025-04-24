namespace Damas.Combat
{
    /// Layla
    public class AttackCommand : ICommand
    {
        private readonly Piece attacker;
        private readonly Piece defender;
        private readonly AttackStat clientAttack;
        private readonly HealthStat receiverHealth;

        public AttackCommand(Piece attacker, Piece defender)
        {
            this.attacker = attacker;
            this.defender = defender;
            
            this.clientAttack = attacker.Attack;
            this.receiverHealth = defender.Health;
        }

        // public bool WouldKill()
        // {
        //     return clientAttack.CurrentValue >= receiverHealth.CurrentValue;
        // }

        public void Execute()
        {
            // checking for rooks
            int finalDamage = clientAttack.CurrentValue;
            
                // check if defender gets shielded
            finalDamage = Rook.ApplyRookShieldIfInRange(defender, finalDamage);
            finalDamage = Knight.ApplyKnightBodyguardIfInRange(defender, finalDamage);

            receiverHealth.ReceiveDamage(finalDamage);


            if (receiverHealth.CurrentValue <= 0)
            {
                defender.OnKilledBy(attacker);
            }
        }
    }
}
