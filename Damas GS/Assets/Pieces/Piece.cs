using Damas.Combat;
using Damas.UI;
using Damas.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
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

        private SpriteRenderer sRenderer;

        public HealthStat Health { get; private set; }
        public AttackStat Attack { get; private set; }
        public List<Piece> Captures { get; private set; } = new();
        public bool IsLocked { get; set; } = false;

        // Current position on the board
        private int boardX;
        private int boardY;

        public int X => boardX;
        public int Y => boardY;
        public Vector2Int BoardKey
        {
            get
            {
                return new(boardX, boardY);
            }
            private set
            {
                boardX = value.x;
                boardY = value.y;
            }
        }

        [field: ReadOnly] public bool IsRegistered { get; private set; }
        [field: ReadOnly] public bool HasMoved { get; private set; }
        public bool IsMyTurn => BoardManager.Instance.CurrentPlayerColor == color;
        [field: ReadOnly] public bool IsSelected { get; private set; }
        [field: ReadOnly] public PieceInfoWindow InfoWindow { get; private set; }

        public float OffsetY => sRenderer.sprite.bounds.extents.y;

        protected int ForwardDir => color == PieceColor.White ? 1 : -1;


        public event System.Action<Piece> BeenCaptured;

        private void Awake()
        {
            sRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            SubscribeOnEnable();
        }


        private void Update()
        {
            log.print(
                $"{gameObject.name}" +
                $"| Health: {Health.CurrentValue} Attack: {Attack.CurrentValue}");

            if (Health.CurrentValue <= 0)
            {
                OnCapture(null);
            }
        }

        public virtual void OnSpawn(Vector2Int spawnCoords)
        {
            WaitThenDo waitForBoard = new(
                this,
                () => BoardManager.Instance.Initialized,
                () => MoveTo(spawnCoords),
                5f,
                () => { }
            );

            waitForBoard.Start();
            Health = new(maxHealth);
            Attack = new(defaultAttack);
            log.print($"OnSpawn Called for {name} at {spawnCoords}");
            HasMoved = false;
        }

        public void SetBoardKey(Vector2Int pos)
        {
            BoardKey = pos;
        }

        public void MoveTo(Vector2Int newPos)
        {
            string msg = "";

            if (IsRegistered)
            {
                BoardManager.Instance.GetTile(this).ClearOverlay();
                BoardManager.Instance.OverlaysOff(GetValidMoves());
                if (!BoardManager.Instance.DeregisterPiece(this, out msg))
                {
                    log.error(msg);
                }
                
                HasMoved = true;
            }

            SetBoardKey(newPos);

            if (!BoardManager.Instance.RegisterPiece(this, out msg))
            {
                log.error(msg);
                return;
            }
            else if (msg.Length > 0)
            {
                log.print(msg);
            }

            IsRegistered = true;
            transform.position = new Vector2(newPos.x, newPos.y - OffsetY);

            if (BoardManager.Instance.Initialized)
            {
                OnAfterMove();
            }
        }

        /// <summary>
        /// for the bishop and king
        /// </summary>
        protected virtual void OnAfterMove()
        {
            
        }

        public virtual void OnCapture(Piece killer)
        {
            BeenCaptured?.Invoke(this);
        }

        public virtual List<Vector2Int> GetValidMoves()
        {
            // If locked, no moves
            if (IsLocked) return new List<Vector2Int>();

            //else just do normal moves
            return GetValidMovesInternal();
        }


        public virtual void Select()
        {
            IsSelected = true;

            if (IsMyTurn)
            {
                BoardManager.Instance.OverlaysOn(GetValidMoves());
            }

            PieceInfoWindowData windowData = new(
                Camera.main,
                transform.position + new Vector3(0f, OffsetY * 2, 0f),
                this
            );

            UiManager.Instance.PieceInfoUI.RequestOpenWindow(windowData);
        }

        public virtual void Deselect()
        {
            IsSelected = false;

            if (InfoWindow != null)
            {
                UiManager.Instance.PieceInfoUI.RequestCloseWindow(InfoWindow);
            }
        }

        protected abstract List<Vector2Int> GetValidMovesInternal();

        private void SubscribeOnEnable()
        {
            UiManager.Instance.PieceInfoUI.WindowOpened += HandlePieceWindowOpened;
        }

        private void UnsubscribeOnDisable()
        {
            UiManager.Instance.PieceInfoUI.WindowOpened -= HandlePieceWindowOpened;
        }

        private void HandlePieceWindowOpened(PieceInfoWindow window)
        {
            if (window.Piece == this)
            {
                InfoWindow = window;
            }
        }

        private void OnDisable()
        {
            UnsubscribeOnDisable();
        }
    } 
}
