using Damas.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    public class Rook : Piece
    {
        private List<Piece> currentlyBuffedAllies = new();
        public override List<Vector2Int> GetValidMovesInternal()
        {
            List<Vector2Int> moves = new();
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, 0, +1));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, 0, -1));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, -1, 0));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, +1, 0));
            return moves;
        }
        private void Start()
        {
            ApplyBuffToNearbyAllies();
        }
        
        protected override void Update()
        {
            base.Update();
            // Remove old buffs
            RemoveBuffFromAllies();
            ApplyBuffToNearbyAllies();
        }
        
        
        public void RemoveBuffFromAllies()
        {
            if (currentlyBuffedAllies != null)
            {
                foreach (Piece ally in currentlyBuffedAllies)
                {
                    ally.HasRookBodyguard = false;
                }
            }
            currentlyBuffedAllies.Clear();
        }

        
        public void ApplyBuffToNearbyAllies()
        {
            int buffRange = 1;

            for (int dx = -buffRange; dx <= buffRange; dx++)
            {
                for (int dy = -buffRange; dy <= buffRange; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int tx = X + dx;
                    int ty = Y + dy;
                    Vector2Int pos = new(tx, ty);
                    if (!BoardManager.Instance.IsInBounds(pos)) continue;

                    if (BoardManager.Instance.TryGetOccupant(
                            BoardManager.Instance.tiles[pos],
                            out Piece occupant))
                    {
                        if (occupant != null && occupant.color == this.color)
                        {
                            currentlyBuffedAllies.Add(occupant);
                            occupant.HasRookBodyguard = true;
                        }
                    }
                }
            }
        }

    }
}