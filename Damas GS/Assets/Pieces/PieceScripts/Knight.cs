using Damas.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    public class Knight : Piece
    {
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
        
        public static int ApplyKnightBodyguardIfInRange(Piece defender, int incomingDamage)
        {
            int shareRange = 1; 
          
            foreach (var kvp in BoardManager.Instance.pieces)
            {
                var piece = kvp.Value;
                if (piece == null) continue;
                if (piece is Knight knight && knight.color == defender.color)
                {
                    // check range
                    int dist = Mathf.Abs(knight.X - defender.X) + Mathf.Abs(knight.Y - defender.Y);
                    if (dist <= shareRange)
                    {
                        
                        int half = incomingDamage / 2;
                        
                        knight.Health.ReceiveDamage(half);
                        
                        return (incomingDamage - half);
                    }
                }
            }

            return incomingDamage; 
        }

    }
}