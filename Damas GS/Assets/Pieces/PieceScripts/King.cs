using Damas.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    public class King : Piece
    {
        [Header("Abilities")]
        [SerializeField] private Vector2Int buffDistance;
        [SerializeField] private int buffAmount;
        [SerializeField] private bool buffSelf;

        private List<Piece> currentlyBuffedAllies = new();

        public override void Deselect()
        {
            base.Deselect();
            RemoveBuffFromAllies();
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
                }
            }
            currentlyBuffedAllies.Clear();
        }

        protected override List<Vector2Int> GetValidMovesInternal()
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
        
        protected override void OnAfterMove()
        {
            base.OnAfterMove();

            // Remove old buffs
            RemoveBuffFromAllies();

          
            ApplyBuffToNearbyAllies();
        }


        private void ApplyBuffToNearbyAllies()
        {


            //for (int dx = -buffDistance; dx <= buffDistance; dx++)
            //{
            //    for (int dy = -buffDistance; dy <= buffDistance; dy++)
            //    {
            //        if (dx == 0 && dy == 0) continue;
            //        int tx = X + dx;
            //        int ty = Y + dy;
            //        Vector2Int pos = new(tx, ty);
            //        if (!BoardManager.Instance.IsInBounds(pos)) continue;

            //        if (BoardManager.Instance.TryGetOccupant(
            //                BoardManager.Instance.tiles[pos],
            //                out Piece occupant))
            //        {
            //            if (occupant != null && occupant.color == this.color)
            //            {
            //                occupant.Attack.ModifyBy(buffAmount);
            //                currentlyBuffedAllies.Add(occupant);
            //            }
            //        }
            //    }
            //}


            for (int columnToCheck = -buffDistance.x; columnToCheck <= buffDistance.x; columnToCheck++)
            {
                for (int rowToCheck = -buffDistance.y; rowToCheck <= buffDistance.y; rowToCheck++)
                {
                    Vector2Int posToCheckLocal = new(columnToCheck, rowToCheck);
                    Vector2Int checkKey = BoardKey + posToCheckLocal;

                    if (!buffSelf && posToCheckLocal == Vector2.zero) { continue; }

                    if (!BoardManager.Instance.TryGetPiece(checkKey, out Piece occupant)) { continue; }

                    if (occupant.color == this.color)
                    {
                        occupant.Health.ReceiveHeal(buffAmount);
                    }
                }
            }
        }

    }
}