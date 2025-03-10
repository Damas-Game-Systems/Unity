using Damas.Combat;
using Damas.Utils;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
public enum PieceColor { White, Black }

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public abstract class Piece : MonoBehaviour, ISelectable
{
    [SerializeField] private dbug log = new();

    public PieceType type;
    public PieceColor color;

    [SerializeField] private int maxHealth;
    [SerializeField] private int defaultAttack;

    [SerializeField] private PieceInfoPanel infoPanel;

    private SpriteRenderer sRenderer;

    public HealthStat Health { get; private set; }
    public AttackStat Attack { get; private set; }
    public List<Piece> Captures { get; private set; } = new();

    // Current position on the board
    private int boardX;
    private int boardY;

    public int X => boardX;
    public int Y => boardY;

    [field: ReadOnly] public bool IsRegistered { get; private set; }
    [field: ReadOnly] public bool HasMoved { get; private set; }
    [field: ReadOnly] public bool IsSelected { get; private set; }

    public float OffsetY => sRenderer.sprite.bounds.extents.y;


    public event System.Action<Piece> BeenCaptured;

    private void Awake()
    {
        sRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        infoPanel?.Initialize(this);
    }


    private void Update()
    {
        log.print(
            $"{gameObject.name}" +
            $"| Health: {Health.CurrentValue} Attack: {Attack.CurrentValue}");

        if (Health.CurrentValue <= 0)
        {
            OnCapture();
        }
    }

    public virtual void OnSpawn(Vector2Int spawnCoords)
    {
        MoveTo(spawnCoords);

        Health = new(maxHealth);
        Attack = new(defaultAttack);

        HasMoved = false;
    }

    public Vector2Int GetPositionData()
    {
        return new(boardX, boardY);
    }

    public void SetBoardIndex(Vector2Int pos)
    {
        boardX = pos.x; boardY = pos.y;
    }

    public void MoveTo(Vector2Int newPos)
    {
        string errorMsg = "";

        if (IsRegistered)
        {
            BoardManager.Instance.DeregisterPiece(this, out errorMsg);
            HasMoved = true;
        }

        SetBoardIndex(newPos);

        if (!BoardManager.Instance.RegisterPiece(this, out errorMsg))
        {
            log.error(errorMsg);
            return;
        }

        IsRegistered = true;
        transform.position = new Vector2(newPos.x, newPos.y - OffsetY);
    }

    protected virtual void OnCapture()
    {
        BeenCaptured?.Invoke(this);
    }
    
    public abstract List<Vector2Int> GetValidMoves();

    public void Select()
    {
        IsSelected = true;
    }

    public void Deselect()
    {
        IsSelected = false;
    }

    //// This replaces OnMouseDown()
    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log($"Clicked on {name} (color = {color}) at boardX={X}, boardY={Y}");
    //    BoardManager.Instance.OnPieceClicked(this);
    //}
}
