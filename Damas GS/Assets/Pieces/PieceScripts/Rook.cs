using Damas.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    public class Rook : Piece
    {
        public override List<Vector2Int> GetValidMoves()
        {
            List<Vector2Int> moves = new();
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, 0, +1));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, 0, -1));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, -1, 0));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, +1, 0));
            return moves;
        }
    }
}