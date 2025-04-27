using Damas.UI;
using UnityEngine;
using UnityEngine.EventSystems; // <-- Add this namespace

namespace Damas
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Tile : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private GameObject emptyTileOverlayPrefab;
        [SerializeField] private GameObject occupiedTileOverlayPrefab;
        [SerializeField] private GameObject selectedTileOverlayPrefab;

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

        public void OnSpawn(Vector2Int spawnPos, Sprite sprite)
        {
            SetPositionData(spawnPos);
            rend.sprite = sprite;
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

        public void SetOverlaySelected()
        {
            overlay = Instantiate(selectedTileOverlayPrefab, transform);
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
}
