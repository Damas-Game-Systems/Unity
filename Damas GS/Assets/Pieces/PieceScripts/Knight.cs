using System;
using Damas.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    public class Knight : Piece
    {
        private List<Piece> currentlyBuffedAllies = new();
        public override List<Vector2Int> GetValidMovesInternal()
        {
            List<Vector2Int> moves = new();
            int[] dx = { +2, +2, -2, -2, +1, +1, -1, -1 };
            int[] dy = { +1, -1, +1, -1, +2, -2, +2, -2 };

            for (int i = 0; i < dx.Length; i++)
            {
                Vector2Int pos = new(X + dx[i], Y + dy[i]);
                if (BoardManager.Instance.IsValidMove(this, pos))
                {
                    moves.Add(pos);
                }
            }
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
                  ally.KnightBodyguards.Remove(this);
                }
            }
            currentlyBuffedAllies.Clear();
        }

        
        public void ApplyBuffToNearbyAllies()
        {
            int buffRange = 1;
            int buffAmount = 2;

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
                            occupant.KnightBodyguards.Add(this);
                        }
                    }
                }
            }
        }

    }
}