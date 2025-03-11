using Damas.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    public class Rook : Piece
    {
        public override List<Vector2Int> GetValidMovesInternal()
        {
            List<Vector2Int> moves = new();
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, 0, +1));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, 0, -1));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, -1, 0));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, +1, 0));
            return moves;
        }
        
        public static int ApplyRookShieldIfInRange(Piece defender, int incomingDamage)
        {
            int shieldRange = 2;  
            int shieldValue = 2;  

            // Scan board for any friendly Rook in range
            //using var here because idk how to reference value pairs in dict
            //kvp stands for key value pair which is what it should be
            foreach (var kvp in BoardManager.Instance.pieces)
            {
                var piece = kvp.Value;
                if (piece == null) continue;
                if (piece is Rook rook && rook.color == defender.color)
                {
                    
                    int dist = Mathf.Abs(rook.X - defender.X) + Mathf.Abs(rook.Y - defender.Y);
                    if (dist <= shieldRange)
                    {
                        
                        int reduced = Mathf.Max(0, incomingDamage - shieldValue);
                        return reduced;
                    }
                }
            }

            return incomingDamage;
        }

    }
}