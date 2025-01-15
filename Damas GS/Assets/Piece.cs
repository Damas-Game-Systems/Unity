using UnityEngine;

public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
public enum PieceColor { White, Black }

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Piece : MonoBehaviour
{
    public PieceType type;
    public PieceColor color;

    // Current position on the board
    public int boardX;
    public int boardY;

    private void OnMouseDown()
    {
        // Forward the click event to the BoardManager
        // so it can select / handle this piece.
        Debug.Log("Clicked on " + name + " with color=" + color + " at boardX=" + boardX + ", boardY=" + boardY);
        BoardManager.Instance.OnPieceClicked(this);
    }
}
