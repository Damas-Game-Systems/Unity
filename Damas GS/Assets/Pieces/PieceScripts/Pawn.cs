using Damas.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    public class Pawn : Piece
    {
        public override List<Vector2Int> GetValidMoves()
        {
            List<Vector2Int> moves = new();
            int direction = color == PieceColor.White ? +1 : -1;

            int x = X;
            int y = Y;
            Vector2Int forward = new(x, y + direction);

            if (BoardManager.Instance.IsTileEmpty(forward))
            {
                moves.Add(forward);
                if (!HasMoved)
                {
                    Vector2Int doubleMove = new(x, y + 2 * direction);
                    if (BoardManager.Instance.IsTileEmpty(doubleMove))
                    {
                        moves.Add(doubleMove);
                    }
                }
            }

            // Diagonal captures
            foreach (int dx in new[] { -1, 1 })
            {
                Vector2Int diag = new(x + dx, y + direction);
                if (BoardManager.Instance.IsOpponentPiece(this, diag))
                {
                    moves.Add(diag);
                }
            }

            return moves;
        }

    }
}