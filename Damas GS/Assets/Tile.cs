using Damas.UI;
using UnityEngine;
using UnityEngine.EventSystems; // <-- Add this namespace

namespace Damas
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Tile : MonoBehaviour, IPointerDownHandler, ISelectable
    {
        // Colors 
        public Color lightColor = Color.white;
        public Color darkColor = Color.gray;
        public Color defaultColor = Color.white;
        public Color highlightColor = Color.green;  // highlight

        [SerializeField] private GameObject emptyTileOverlayPrefab;
        [SerializeField] private GameObject occupiedTileOverlayPrefab;
        [SerializeField] private GameObject selectedTileOverlayPrefab;

        [SerializeField, ReadOnly] private Piece previousOccupant;

        private GameObject overlay;

        private SpriteRenderer rend;

        private int boardX;  // column
        private int boardY;  // row

        public int X { get { return boardX; } }
        public int Y { get { return boardY; } }
        public Vector2Int BoardKey
        {
            get
            {
                return new (boardX, boardY);
            }
            private set
            {
                boardX = value.x;
                boardY = value.y;
            }
        }

        [field: ReadOnly] public Piece Occupant { get; private set; }
        public bool IsEmpty => Occupant == null;

        [field: ReadOnly] public bool IsSelected { get; private set; }



        private void Awake()
        {
            rend = GetComponent<SpriteRenderer>();
        }

        // This replaces OnMouseDown()
        public void OnPointerDown(PointerEventData eventData)
        {
            BoardManager.Instance.OnTileClicked(this);
        }

        public void OnSpawn(Vector2Int spawnPos)
        {
            SetBoardKey(spawnPos);
        }

        public void SetBoardKey(Vector2Int pos)
        {
            BoardKey = pos;
        }

        public void AddPiece(Piece piece)
        {
            previousOccupant = Occupant;
            Occupant = piece;
        }

        public void RemovePiece()
        {
            Occupant?.Deselect();
            Occupant = null;
        }


        public void SetOverlay()
        {
            ClearOverlay();

            if (IsSelected)
            {
                overlay = Instantiate(selectedTileOverlayPrefab, transform);
            }
            else if (IsEmpty)
            {
                overlay = Instantiate(emptyTileOverlayPrefab, transform);
            }
            else if (!Occupant.IsMyTurn)
            {
                overlay = Instantiate(occupiedTileOverlayPrefab, transform);
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

        public void Select()
        {
            IsSelected = true;
            SetOverlay();
            Occupant?.Select();
        }

        public void Deselect()
        {
            IsSelected = false;
            ClearOverlay();
            Occupant?.Deselect();
        }
    }
}