using Damas.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    public class King : Piece
    {
        public override List<Vector2Int> GetValidMoves()
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
    }
}