using UnityEngine;
using UnityEngine.EventSystems; // <-- Add this namespace

public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
public enum PieceColor { White, Black }

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Piece : MonoBehaviour, IPointerDownHandler
{
    public PieceType type;
    public PieceColor color;
    public int MaxHealth;
    public int currentHealth;
    public int atkPower;

    // Current position on the board
    public int boardX;
    public int boardY;

    // This replaces OnMouseDown()
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"Clicked on {name} (color = {color}) at boardX={boardX}, boardY={boardY}");
        BoardManager.Instance.OnPieceClicked(this);
    }
}
