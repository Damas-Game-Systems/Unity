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
        [Header("Abilities")]
        [SerializeField] private Vector2Int healRange;
        [SerializeField] private int healAmount;
        [SerializeField] private bool healsSelf;
        
        protected override List<Vector2Int> GetValidMovesInternal()
        {
            List<Vector2Int> moves = new();
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, new(+1, +1)));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, new(+1, -1)));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, new(-1, +1)));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, new(-1, -1)));
            return moves;
        }

        protected override void OnAfterMove()
        {
            base.OnAfterMove();
            HealNearbyAllies();
        }

        private void HealNearbyAllies()
        {
            for (int columnToCheck = -healRange.x; columnToCheck <= healRange.x; columnToCheck++)
            {
                for (int rowToCheck = -healRange.y; rowToCheck <= healRange.y; rowToCheck++)
                {
                    Vector2Int posToCheckLocal = new(columnToCheck, rowToCheck);
                    Vector2Int checkKey = BoardKey + posToCheckLocal;

                    if (!healsSelf && posToCheckLocal == Vector2.zero) { continue; }

                    if (!BoardManager.Instance.TryGetPiece(checkKey, out Piece occupant)) { continue; }

                    if (occupant.color == this.color)
                    {
                        occupant.Health.ReceiveHeal(healAmount);
                    }
                }
            }
        }
    }
}