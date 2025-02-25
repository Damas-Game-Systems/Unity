using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override List<Vector2Int> GetValidMoves()
    {
        List<Vector2Int> moves = new();
        moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, +1, +1));
        moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, +1, -1));
        moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, -1, +1));
        moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, -1, -1));
        return moves;
    }
}