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
            BoardManager.Instance.StartCoroutine(UnlockPieceNextTurn(enemy));
        }

        private System.Collections.IEnumerator UnlockPieceNextTurn(Piece enemy)
        {
            // Wait until the next turn. 
            yield return new UnityEngine.WaitUntil(() => 
                BoardManager.Instance.currentPlayerColor == this.color
            );
           
            if (enemy != null) enemy.IsLocked = false;
        }

    }
}