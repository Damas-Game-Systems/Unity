using Damas.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    public class King : Piece
    {
        private List<Piece> currentlyBuffedAllies = new();
        
        public override List<Vector2Int> GetValidMovesInternal()
        {
            List<Vector2Int> moves = new();
            int[] dx = { -1, 0, +1 };
            int[] dy = { -1, 0, +1 };

            foreach (int ix in dx)
            {
                foreach (int iy in dy)
                {
                    if (ix == 0 && iy == 0) continue;
                    Vector2Int pos = new(X + ix, Y + iy);
                    if (BoardManager.Instance.IsValidMove(this, pos))
                    {
                        moves.Add(pos);
                    }
                }
            }
            return moves;
        }
        
        protected override void Update()
        {
            base.Update();
            
            RemoveBuffFromAllies();

            ApplyBuffToNearbyAllies();
        }

        // E.g. remove buffs at the end of the turn
        // called  from BoardManager.SwitchTurn() 
        public void RemoveBuffFromAllies()
        {
            if (currentlyBuffedAllies != null)
            {
                foreach (Piece ally in currentlyBuffedAllies)
                {
                    ally.Attack.ModifyBy(-2);
                    ally.HasKingBuff = false;
                }
            }
            currentlyBuffedAllies.Clear();
        }

        private void ApplyBuffToNearbyAllies()
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
                            occupant.Attack.ModifyBy(buffAmount);
                            occupant.HasKingBuff = true;
                            currentlyBuffedAllies.Add(occupant);
                        }
                    }
                }
            }
        }
    }
}
