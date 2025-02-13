using System;
using UnityEngine;
using UnityEngine.EventSystems; // <-- Add this namespace

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour, IPointerDownHandler
{
    // Colors 
    public Color lightColor = Color.white;
    public Color darkColor = Color.gray;
    public Color defaultColor = Color.white;
    public Color highlightColor = Color.green;  // highlight

    [SerializeField] private GameObject emptyTileOverlayPrefab;
    [SerializeField] private GameObject occupiedTileOverlayPrefab;

    private GameObject overlay;

    private SpriteRenderer rend;

    private int boardX;  // column
    private int boardY;  // row

    public int X { get { return boardX; } }
    public int Y { get { return boardY; } }

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    public void OnSpawn(Vector2Int spawnPos)
    {
        SetPositionData(spawnPos);
    }

    public Vector2Int GetPositionData()
    {
        return new Vector2Int(boardX, boardY);
    }

    public void SetPositionData(Vector2Int pos)
    {
        boardX = pos.x;
        boardY = pos.y;
    }

    // Sets the tile’s color
    public void SetHighlight(bool highlight)
    {
        rend.color = highlight ? highlightColor : defaultColor;
    }

    // This replaces OnMouseDown()
    public void OnPointerDown(PointerEventData eventData)
    {
        BoardManager.Instance.OnTileClicked(this);
    }

    public void SetOverlay(bool hasPiece)
    {
        ClearOverlay();

        if (hasPiece)
        {
            overlay = Instantiate(occupiedTileOverlayPrefab, transform);
        }
        else
        {
            overlay = Instantiate(emptyTileOverlayPrefab, transform);
        }
    }

    public void ClearOverlay()
    {
        if (overlay != null)
        {
            Destroy(overlay);
        }
        overlay = null;
    }

}
