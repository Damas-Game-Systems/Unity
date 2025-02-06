using UnityEngine;
using UnityEngine.EventSystems; // <-- Add this namespace

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour, IPointerDownHandler
{
    public int boardX;  // column
    public int boardY;  // row

    // Colors 
    public Color lightColor = Color.white;
    public Color darkColor = Color.gray;
    public Color defaultColor = Color.white;
    public Color highlightColor = Color.green;  // highlight

    private SpriteRenderer rend;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        BoardManager.Instance.RegisterTile(this, boardX, boardY);
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
}
