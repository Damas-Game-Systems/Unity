using Damas.Combat;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // <-- Add this namespace

public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
public enum PieceColor { White, Black }

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Piece : MonoBehaviour
{
    public PieceType type;
    public PieceColor color;

    [SerializeField] private int maxHealth;
    [SerializeField] private int defaultAttack;

    private HealthStat health;
    private AttackStat attack;

    private List<Piece> captures;


    // Current position on the board
    private int boardX;
    private int boardY;

    public int X { get { return boardX; } }
    public int Y { get { return boardY; } }
    public bool HasMoved { get; private set; }

    public void OnSpawn(Vector2Int spawnCoords)
    {
        SetPositionData(spawnCoords);
        HasMoved = false;
    }

    public Vector2Int GetPositionData()
    {
        return new(boardX, boardY);
    }

    public void SetPositionData(Vector2Int pos)
    {
        boardX = pos.x; boardY = pos.y;
    }

    public void MoveTo(Vector2Int newPos)
    {
        SetPositionData(newPos);
        BoardManager.Instance.DeregisterPiece(this);
        BoardManager.Instance.RegisterPiece(this);
        transform.position = (Vector2)newPos;
        HasMoved = true;
    }

    //// This replaces OnMouseDown()
    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log($"Clicked on {name} (color = {color}) at boardX={X}, boardY={Y}");
    //    BoardManager.Instance.OnPieceClicked(this);
    //}
}
