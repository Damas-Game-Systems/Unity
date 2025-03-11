using Damas.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    /// <summary>
    /// Ability: Heals nearby allies
    /// </summary>
    public class Bishop : Piece
    {
        
        public override List<Vector2Int> GetValidMovesInternal()
        {
            List<Vector2Int> moves = new();
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, +1, +1));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, +1, -1));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, -1, +1));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, -1, -1));
            return moves;
        }

        protected override void OnAfterMove()
        {
            base.OnAfterMove();
            HealNearbyAllies();
        }

        private void HealNearbyAllies()
        {
            int healRange = 1;
            int healAmount = 2;

            for (int dx = -healRange; dx <= healRange; dx++)
            {
                for (int dy = -healRange; dy <= healRange; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    int tileX = X + dx;
                    int tileY = Y + dy;
                    Vector2Int pos = new Vector2Int(tileX, tileY);

                    if (!BoardManager.Instance.IsInBounds(pos))
                    {
                        continue;
                    }

                    if (BoardManager.Instance.TryGetOccupant(BoardManager.Instance.tiles[pos], out Piece occupant))
                    {
                        if (occupant != null && occupant.color == this.color)
                        {
                            occupant.Health.ReceiveHeal(healAmount);
                        }
                    }
                        
                }
            }
        }
    }
}