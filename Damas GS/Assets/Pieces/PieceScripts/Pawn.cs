using Damas.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    /// <summary>
    /// Ability: Counterattack when killed
    /// </summary>
    public class Pawn : Piece
    {
        protected override List<Vector2Int> GetValidMovesInternal()
        {
            List<Vector2Int> moves = new();

            int x = X;
            int y = Y;

            Func<int, Vector2Int> forwardPos =
                (int distance) => BoardKey + new Vector2Int(0, ForwardDir * distance);

            if (BoardManager.Instance.IsEmptyAt(forwardPos(1)))
            {
                moves.Add(forwardPos(1));

                if (!HasMoved && !BoardManager.Instance.IsEmptyAt(forwardPos(2)))
                {
                    moves.Add(forwardPos(2));
                }
            }

            // Diagonal captures
            foreach (int dx in new[] { -1, 1 })
            {
                Vector2Int diag = new(x + dx, y + ForwardDir);
                if (BoardManager.Instance.IsOpponentAt(diag, this))
                {
                    moves.Add(diag);
                }
            }

            return moves;
        }

        public override void OnCapture(Piece killer)
        {
            if (killer != null)
            {
                killer.Health.ReceiveDamage(this.Attack.CurrentValue);
                
            }
            base.OnCapture(killer);
           
        }

    }
}