using Damas.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    public class Queen : Piece
    {
        public override List<Vector2Int> GetValidMovesInternal()
        {
            List<Vector2Int> moves = new();
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, 0, +1));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, 0, -1));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, -1, 0));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, +1, 0));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, +1, +1));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, +1, -1));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, -1, +1));
            moves.AddRange(BoardManager.Instance.GetMovesInDirection(this, -1, -1));
            return moves;
        }
        
        public void LockEnemy(Piece enemy)
        {
            if (enemy == null || enemy.color == this.color) return;

            enemy.IsLocked = true;

            //unlocking mechanism after 1 turn:
            StartCoroutine(UnlockPieceNextTurn(enemy));
        }

        private System.Collections.IEnumerator UnlockPieceNextTurn(Piece enemy)
        {
            
            PieceColor initialPlayerColor = BoardManager.Instance.currentPlayerColor;
            int turnChanges = 0;

            
            yield return new UnityEngine.WaitUntil(() =>
            {
                if (BoardManager.Instance.currentPlayerColor != initialPlayerColor)
                {
                    initialPlayerColor = BoardManager.Instance.currentPlayerColor;
                    turnChanges++;
                }
                return turnChanges >= 2;
            });

           
            if (enemy != null) enemy.IsLocked = false;
        }

    }
}
