using Damas.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    public class Knight : Piece
    {
        public override List<Vector2Int> GetValidMoves()
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
    }
}